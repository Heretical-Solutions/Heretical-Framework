using System;
using System.Collections.Generic;

namespace HereticalSolutions.Repositories.Factories
{
    public class RepositoryFactory
    {
        #region Dictionary object repository

        public DictionaryInstanceRepository
            BuildDictionaryInstanceRepository()
        {
            return new DictionaryInstanceRepository(
                BuildDictionaryRepository<Type, object>(),
                this);
        }
        
        public DictionaryInstanceRepository
            BuildDictionaryInstanceRepository(
                IRepository<Type, object> database)
        {
            return new DictionaryInstanceRepository(
                database,
                this);
        }

        public DictionaryInstanceRepository
            CloneDictionaryInstanceRepository(
                IRepository<Type, object> contents)
        {
            return new DictionaryInstanceRepository(
                ((IClonableRepository<Type, object>)contents).Clone(),
                this);
        }

        #endregion
        
        #region Dictionary repository
        
        public DictionaryRepository<TKey, TValue>
            BuildDictionaryRepository<TKey, TValue>()
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(),
                this);
        }
        
        public DictionaryRepository<TKey, TValue>
            BuildDictionaryRepository<TKey, TValue>(
                Dictionary<TKey, TValue> database)
        {
            return new DictionaryRepository<TKey, TValue>(
                database,
                this);
        }
        
        public DictionaryRepository<TKey, TValue>
            BuildDictionaryRepository<TKey, TValue>(
                IEqualityComparer<TKey> comparer)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(comparer),
                this);
        }
        
        public DictionaryRepository<TKey, TValue>
            CloneDictionaryRepository<TKey, TValue>(
                Dictionary<TKey, TValue> contents)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(contents),
                this);
        }

        #endregion
    }
}