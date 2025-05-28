using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using HereticalSolutions.Repositories.Concurrent.Factories;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Repositories.Concurrent
{
	public class ConcurrentDictionaryRepository<TKey, TValue> :
		IRepository<TKey, TValue>,
		IClonableRepository<TKey, TValue>,
		ICleanuppable,
		IDisposable
	{
		private readonly ConcurrentDictionary<TKey, TValue> database;

		private readonly ConcurrentRepositoryFactory concurrentRepositoryFactory;

		public ConcurrentDictionaryRepository(
			ConcurrentDictionary<TKey, TValue> database,
			ConcurrentRepositoryFactory concurrentRepositoryFactory)
		{
			this.database = database;

			this.concurrentRepositoryFactory = concurrentRepositoryFactory;
		}

		#region IRepository

		#region IReadOnlyRepository

		public bool Has(
			TKey key)
		{
			return database.ContainsKey(key);
		}

		public TValue Get(
			TKey key)
		{
			return database[key];
		}

		public bool TryGet(
			TKey key,
			out TValue value)
		{
			return database.TryGetValue(
				key,
				out value);
		}

		public int Count { get { return database.Count; } }

		public IEnumerable<TKey> Keys { get { return database.Keys; } }

		public IEnumerable<TValue> Values { get { return database.Values; } }

		#endregion

		public void Add(
			TKey key,
			TValue value)
		{
			database.TryAdd(
				key,
				value);
		}

		public bool TryAdd(
			TKey key,
			TValue value)
		{
			return database.TryAdd(
				key,
				value);
		}

		public TValue this[TKey key]
		{
			get
			{
				return database[key];
			}
			set
			{
				database[key] = value;
			}
		}

		public void Update(
			TKey key,
			TValue value)
		{
			database[key] = value;
		}

		public bool TryUpdate(
			TKey key,
			TValue value)
		{
			return database.TryUpdate(
				key,
				value,
				database[key]);
		}

		public void AddOrUpdate(
			TKey key,
			TValue value)
		{
			database.AddOrUpdate(
				key,
				value,
				(k, v) => value);
		}

		public void Remove(
			TKey key)
		{
			database.TryRemove(
				key,
				out var value);
		}

		public bool TryRemove(
			TKey key)
		{
			return database.TryRemove(
				key,
				out var value);
		}


		public void Clear()
		{
			database.Clear();
		}

		#endregion

		#region IClonableRepository

		public IRepository<TKey, TValue> Clone()
		{
			return concurrentRepositoryFactory
				.CloneConcurrentDictionaryRepository(database);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var value in database.Values)
			{
				if (value is ICleanuppable)
					(value as ICleanuppable).Cleanup();
			}

			Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var value in database.Values)
			{
				if (value is IDisposable)
					(value as IDisposable).Dispose();
			}

			Clear();

			if (database is IDisposable)
				(database as IDisposable).Dispose();
		}

		#endregion
	}
}