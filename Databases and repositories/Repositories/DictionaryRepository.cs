using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Persistence;

namespace HereticalSolutions.Repositories
{
    public class DictionaryRepository<TKey, TValue> :
        IRepository<TKey, TValue>,
        IClonableRepository<TKey, TValue>,
        IVisitable,
        ICleanuppable,
        IDisposable
    {
        private readonly Dictionary<TKey, TValue> database;

        private readonly RepositoryFactory repositoryFactory;

        public DictionaryRepository(
            Dictionary<TKey, TValue> database,
            RepositoryFactory repositoryFactory)
        {
            this.database = database;

            this.repositoryFactory = repositoryFactory;
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
            database.Add(
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
            if (!Has(key))
                return false;

            Update(
                key,
                value);

            return true;
        }

        public void AddOrUpdate(
            TKey key,
            TValue value)
        {
            if (Has(key))
                Update(
                    key,
                    value);
            else
                Add(
                    key,
                    value);
        }

        public void Remove(
            TKey key)
        {
            database.Remove(key);
        }

        public bool TryRemove(
            TKey key)
        {
            if (!Has(key))
                return false;

            Remove(key);

            return true;
        }


        public void Clear()
        {
            database.Clear();
        }

        #endregion

        #region IClonableRepository

        public IRepository<TKey, TValue> Clone()
        {
            return repositoryFactory.CloneDictionaryRepository(
                database);
        }

        #endregion

        #region IVisitable

        public bool AcceptSave(
            ISaveVisitor visitor,
            ref object dto)
        {
            return visitor.VisitSave<DictionaryRepository<TKey, TValue>>(
                ref dto,
                this,
                visitor);
        }

        public bool AcceptPopulate(
            IPopulateVisitor visitor,
            object dto)
        {
            return visitor.VisitPopulate<DictionaryRepository<TKey, TValue>>(
                dto,
                this,
                visitor);
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