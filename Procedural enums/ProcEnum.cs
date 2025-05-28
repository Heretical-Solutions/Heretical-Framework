using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Persistence;

namespace HereticalSolutions.ProceduralEnums
{
	public class ProcEnum<TValue>
		: IEquatable<ProcEnum<TValue>>
	{
		private readonly IReadOnlyOneToOneMap<int, TValue> map;

		private int index;

		public ProcEnum(
			IReadOnlyOneToOneMap<int, TValue> map,
			int index = 0)
		{
			this.map = map;

			this.index = index;
		}

		public int Index
		{
			get => index;
			set => index = value;
		}

		// Check if bit at index i is set (returns true if 1, false if 0)
		public bool IsBitSet(int i) => (index & (1 << i)) != 0;

		// Set bit at index i to 1
		public int SetBit(int i) => index | (1 << i);

		// Clear bit at index i (set to 0)
		public int ClearBit(int i) => index & ~(1 << i);

		// Toggle bit at index i (flip between 0 and 1)
		public int ToggleBit(int i) => index ^ (1 << i);

		public TValue Value
		{
			get
			{
				if (!map.TryGetRight(
					index,
					out TValue value))
				{
					throw new ArgumentOutOfRangeException(
						nameof(index),
						$"Index {index} is out of range.");
				}

				return value;
			}
			set
			{
				if (!map.TryGetLeft(
					value,
					out int index))
				{
					throw new ArgumentOutOfRangeException(
						nameof(value),
						$"Value {value} is out of range.");
				}

				this.index = index;
			}
		}

		public IEnumerable<TValue> ValuesAtFlagBits
		{
			get
			{
				foreach (int i in map.LeftValues)
				{
					if (IsBitSet(i))
					{
						yield return map.GetRight(i);
					}
				}
			}
		}

		public IReadOnlyOneToOneMap<int, TValue> Map => map;

		public IEnumerable<int> AllIndexes => map.LeftValues;

		public IEnumerable<TValue> AllValues => map.RightValues;

		public TValue this[int index]
		{
			get
			{
				if (!map.TryGetRight(
					index,
					out TValue value))
				{
					throw new ArgumentOutOfRangeException(
						nameof(index),
						$"Index {index} is out of range.");
				}

				return value;
			}
		}

		public int this[TValue value]
		{
			get
			{
				if (!map.TryGetLeft(
					value,
					out int index))
				{
					throw new ArgumentOutOfRangeException(
						nameof(value),
						$"Value {value} is out of range.");
				}

				return index;
			}
		}

		public bool TryParse(
			TValue value,
			out int index)
		{
			return map.TryGetLeft(
				value,
				out index);
		}

		public static ProcEnum<TValue> operator |(
			ProcEnum<TValue> left,
			ProcEnum<TValue> right)
		{
			if (left.map != right.map)
			{
				throw new ArgumentException(
					$"Cannot combine ProcEnums with different maps. {left.map} != {right.map}");
			}

			return new ProcEnum<TValue>(
				left.map,
				left.index | right.index);
		}

		public static ProcEnum<TValue> operator &(
			ProcEnum<TValue> left,
			ProcEnum<TValue> right)
		{
			if (left.map != right.map)
			{
				throw new ArgumentException(
					$"Cannot combine ProcEnums with different maps. {left.map} != {right.map}");
			}

			return new ProcEnum<TValue>(
				left.map,
				left.index & right.index);
		}

		public static ProcEnum<TValue> operator ^(
			ProcEnum<TValue> left,
			ProcEnum<TValue> right)
		{
			if (left.map != right.map)
			{
				throw new ArgumentException(
					$"Cannot combine ProcEnums with different maps. {left.map} != {right.map}");
			}

			return new ProcEnum<TValue>(
				left.map,
				left.index ^ right.index);
		}

		public static ProcEnum<TValue> operator ~(
			ProcEnum<TValue> left)
		{
			if (left.map == null)
			{
				throw new ArgumentNullException(
					nameof(left.map),
					"Cannot negate a ProcEnum with a null map.");
			}

			return new ProcEnum<TValue>(
				left.map,
				~left.index);
		}

		public static bool operator ==(
			ProcEnum<TValue> left,
			ProcEnum<TValue> right)
		{
			if (left.map == null || right.map == null)
			{
				return false;
			}

			return left.Map == right.map
				&& left.index == right.index;
		}

		public static bool operator !=(
			ProcEnum<TValue> left,
			ProcEnum<TValue> right)
		{
			if (left.map == null || right.map == null)
			{
				return true;
			}

			return left.Map == right.map
				&& left.index != right.index;
		}

		public override bool Equals(
			object obj)
		{
			if (obj is ProcEnum<TValue> other)
			{
				return this == other;
			}

			return false;
		}

		public bool Equals(
			ProcEnum<TValue> other)
		{
			if (other == null)
			{
				return false;
			}

			return this == other;
		}

		public override int GetHashCode()
		{
			return index.GetHashCode() ^ map.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Value}";
		}

		#region IVisitable

		public bool AcceptSave(
			ISaveVisitor visitor,
			ref object dto)
		{
			return visitor.VisitSave<ProcEnum<TValue>>(
				ref dto,
				this,
				visitor);
		}

		public bool AcceptPopulate(
			IPopulateVisitor visitor,
			object dto)
		{
			return visitor.VisitPopulate<ProcEnum<TValue>>(
				dto,
				this,
				visitor);
		}

		#endregion
	}
}