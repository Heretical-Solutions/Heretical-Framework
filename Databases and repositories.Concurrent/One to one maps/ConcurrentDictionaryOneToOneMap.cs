using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Repositories.Concurrent.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories.Concurrent
{
	public class ConcurrentDictionaryOneToOneMap<TValue1, TValue2> :
		IOneToOneMap<TValue1, TValue2>,
		ICleanuppable,
		IDisposable
	{
		private readonly Dictionary<TValue1, TValue2> leftToRightDatabase;

		private readonly Dictionary<TValue2, TValue1> rightToLeftDatabase;

		private readonly ConcurrentOneToOneMapFactory concurrentOneToOneMapFactory;

		private readonly object leftToRightLock;

		private readonly object rightToLeftLock;

		private readonly ILogger logger;

		public ConcurrentDictionaryOneToOneMap(
			Dictionary<TValue1, TValue2> leftToRightDatabase,
			Dictionary<TValue2, TValue1> rightToLeftDatabase,
			ConcurrentOneToOneMapFactory concurrentOneToOneMapFactory,
			object leftToRightLock,
			object rightToLeftLock,
			ILogger logger)
		{
			this.leftToRightDatabase = leftToRightDatabase;

			this.rightToLeftDatabase = rightToLeftDatabase;


			this.concurrentOneToOneMapFactory = concurrentOneToOneMapFactory;


			this.leftToRightLock = leftToRightLock;

			this.rightToLeftLock = rightToLeftLock;


			this.logger = logger;
		}

		#region IOneToOneMap

		#region IReadOnlyOneToOneMap

		public bool HasLeft(
			TValue1 key)
		{
			lock (leftToRightLock)
			{
				return leftToRightDatabase.ContainsKey(key);
			}
		}

		public bool HasRight(
			TValue2 key)
		{
			lock (rightToLeftLock)
			{
				return rightToLeftDatabase.ContainsKey(key);
			}
		}

		public TValue2 GetRight(
			TValue1 key)
		{
			lock (leftToRightLock)
			{
				return leftToRightDatabase[key];
			}
		}

		public TValue1 GetLeft(
			TValue2 key)
		{
			lock (rightToLeftLock)
			{
				return rightToLeftDatabase[key];
			}
		}

		public bool TryGetRight(
			TValue1 key,
			out TValue2 value)
		{
			lock (leftToRightLock)
			{
				return leftToRightDatabase.TryGetValue(
					key,
					out value);
			}
		}

		public bool TryGetLeft(
			TValue2 key,
			out TValue1 value)
		{
			lock (rightToLeftLock)
			{
				return rightToLeftDatabase.TryGetValue(
					key,
					out value);
			}
		}

		public int Count
		{
			get
			{
				lock (leftToRightLock)
				lock (rightToLeftLock)
				{
					if (leftToRightDatabase.Count != rightToLeftDatabase.Count)
						throw new Exception(
							logger.TryFormatException(
								GetType(),
								$"LEFT TO RIGHT MAP COUNT ({leftToRightDatabase.Count}) IS NOT EQUAL TO RIGHT TO LEFT MAP COUNT ({rightToLeftDatabase.Count})"));

					return leftToRightDatabase.Count;
				}
			}
		}

		public IEnumerable<Tuple<TValue1, TValue2>> Values
		{
			get
			{
				lock (leftToRightLock)
				{
					var result = new Tuple<TValue1, TValue2>[Count];

					var keys = leftToRightDatabase.Keys;

					int index = 0;

					foreach (var key in keys)
					{
						result[index] = new Tuple<TValue1, TValue2>(
							key,
							leftToRightDatabase[key]);

						index++;
					}

					return result;
				}
			}
		}

		public IEnumerable<TValue1> LeftValues
		{
			get
			{
				lock (leftToRightLock)
				{
					return leftToRightDatabase.Keys;
				}
			}
		}

		public IEnumerable<TValue2> RightValues
		{
			get
			{
				lock (rightToLeftLock)
				{
					return rightToLeftDatabase.Keys;
				}
			}
		}

		#endregion

		public void Add(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				leftToRightDatabase.Add(
					leftValue,
					rightValue);
				
				rightToLeftDatabase.Add(
					rightValue,
					leftValue);
			}
		}

		public bool TryAdd(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				bool leftAdded = leftToRightDatabase.TryAdd(
					leftValue,
					rightValue);

				bool rightAdded = rightToLeftDatabase.TryAdd(
					rightValue,
					leftValue);

				if (leftAdded && rightAdded)
					return true;

				if (leftAdded && !rightAdded)
					leftToRightDatabase.Remove(leftValue);

				if (!leftAdded && rightAdded)
					rightToLeftDatabase.Remove(rightValue);

				return false;
			}
		}

		public void UpdateByLeft(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				var previousRightValue = leftToRightDatabase[leftValue];

				rightToLeftDatabase.Remove(previousRightValue);

				rightToLeftDatabase.Add(
					rightValue,
					leftValue);
				
				leftToRightDatabase[leftValue] = rightValue;
			}
		}

		public void UpdateByRight(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				var previousLeftValue = rightToLeftDatabase[rightValue];

				leftToRightDatabase.Remove(previousLeftValue);

				leftToRightDatabase.Add(
					leftValue,
					rightValue);

				rightToLeftDatabase[rightValue] = leftValue;
			}
		}

		public bool TryUpdateByLeft(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			{
				if (!HasLeft(leftValue))
					return false;

				UpdateByLeft(
					leftValue,
					rightValue);

				return true;
			}
		}

		public bool TryUpdateByRight(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (rightToLeftLock)
			{
				if (!HasRight(rightValue))
					return false;

				UpdateByRight(
					leftValue,
					rightValue);

				return true;
			}
		}

		public void AddOrUpdateByLeft(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				if (HasLeft(leftValue))
					UpdateByLeft(
						leftValue,
						rightValue);
				else
					Add(
						leftValue,
						rightValue);
			}
		}

		public void AddOrUpdateByRight(
			TValue1 leftValue,
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				if (HasRight(rightValue))
					UpdateByRight(
						leftValue,
						rightValue);
				else
					Add(
						leftValue,
						rightValue);
			}
		}

		public void RemoveByLeft(
			TValue1 leftValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				var previousRightValue = leftToRightDatabase[leftValue];

				rightToLeftDatabase.Remove(previousRightValue);

				leftToRightDatabase.Remove(leftValue);
			}
		}

		public void RemoveByRight(
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				var previousLeftValue = rightToLeftDatabase[rightValue];

				leftToRightDatabase.Remove(previousLeftValue);

				rightToLeftDatabase.Remove(rightValue);
			}
		}

		public bool TryRemoveByLeft(
			TValue1 leftValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				if (!HasLeft(leftValue))
					return false;

				RemoveByLeft(leftValue);

				return true;
			}
		}

		public bool TryRemoveByRight(
			TValue2 rightValue)
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				if (!HasRight(rightValue))
					return false;

				RemoveByRight(rightValue);

				return true;
			}
		}

		public void Clear()
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				leftToRightDatabase.Clear();
				rightToLeftDatabase.Clear();
			}
		}

		#endregion

		#region IClonableOneToOneMap

		public IOneToOneMap<TValue1, TValue2> Clone()
		{
			return concurrentOneToOneMapFactory
				.CloneConcurrentDictionaryOneToOneMap(
					leftToRightDatabase,
					rightToLeftDatabase,
					logger);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				foreach (var value in leftToRightDatabase.Values)
				{
					if (value is ICleanuppable)
						(value as ICleanuppable).Cleanup();
				}

				foreach (var value in rightToLeftDatabase.Values)
				{
					if (value is ICleanuppable)
						(value as ICleanuppable).Cleanup();
				}

				Clear();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (leftToRightLock)
			lock (rightToLeftLock)
			{
				foreach (var value in leftToRightDatabase.Values)
				{
					if (value is IDisposable)
						(value as IDisposable).Dispose();
				}

				foreach (var value in rightToLeftDatabase.Values)
				{
					if (value is IDisposable)
						(value as IDisposable).Dispose();
				}

				Clear();

				if (leftToRightDatabase is IDisposable)
					(leftToRightDatabase as IDisposable).Dispose();

				if (rightToLeftDatabase is IDisposable)
					(rightToLeftDatabase as IDisposable).Dispose();
			}
		}

		#endregion
	}
}