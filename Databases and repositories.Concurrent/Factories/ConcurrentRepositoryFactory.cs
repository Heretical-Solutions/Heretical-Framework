using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace HereticalSolutions.Repositories.Concurrent.Factories
{
	public class ConcurrentRepositoryFactory
	{
		public ConcurrentRepositoryFactory()
		{
		}

		#region Concurrent dictionary object repository

		public ConcurrentDictionaryInstanceRepository
			BuildConcurrentDictionaryInstanceRepository()
		{
			return new ConcurrentDictionaryInstanceRepository(
				new ConcurrentDictionary<Type, object>(),
				this);
		}

		public ConcurrentDictionaryInstanceRepository
			BuildConcurrentDictionaryInstanceRepository(
				ConcurrentDictionary<Type, object> database)
		{
			return new ConcurrentDictionaryInstanceRepository(
				database,
				this);
		}

		public ConcurrentDictionaryInstanceRepository
			BuildConcurrentDictionaryInstanceRepository(
				IEqualityComparer<Type> comparer)
		{
			return new ConcurrentDictionaryInstanceRepository(
				new ConcurrentDictionary<Type, object>(comparer),
				this);
		}

		public ConcurrentDictionaryInstanceRepository
			CloneConcurrentDictionaryInstanceRepository(
				ConcurrentDictionary<Type, object> contents)
		{
			return new ConcurrentDictionaryInstanceRepository(
				new ConcurrentDictionary<Type, object>(contents),
				this);
		}

		#endregion

		#region Concurrent dictionary repository

		public ConcurrentDictionaryRepository<TKey, TValue>
			BuildConcurrentDictionaryRepository<TKey, TValue>()
		{
			return new ConcurrentDictionaryRepository<TKey, TValue>(
				new ConcurrentDictionary<TKey, TValue>(),
				this);
		}

		public ConcurrentDictionaryRepository<TKey, TValue>
			BuildConcurrentDictionaryRepository<TKey, TValue>(
				ConcurrentDictionary<TKey, TValue> database)
		{
			return new ConcurrentDictionaryRepository<TKey, TValue>(
				database,
				this);
		}

		public ConcurrentDictionaryRepository<TKey, TValue>
			BuildConcurrentDictionaryRepository<TKey, TValue>(
				IEqualityComparer<TKey> comparer)
		{
			return new ConcurrentDictionaryRepository<TKey, TValue>(
				new ConcurrentDictionary<TKey, TValue>(comparer),
				this);
		}

		public ConcurrentDictionaryRepository<TKey, TValue>
			CloneConcurrentDictionaryRepository<TKey, TValue>(
				ConcurrentDictionary<TKey, TValue> contents)
		{
			return new ConcurrentDictionaryRepository<TKey, TValue>(
				new ConcurrentDictionary<TKey, TValue>(contents),
				this);
		}

		#endregion
	}
}