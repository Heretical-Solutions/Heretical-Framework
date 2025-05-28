using System;

namespace HereticalSolutions.Repositories
{
    public interface IInstanceRepository
        : IReadOnlyInstanceRepository
    {
        void Add<TValue>(
            TValue value);

        void Add(
            Type valueType,
            object value);

        bool TryAdd<TValue>(
            TValue value);

        bool TryAdd(
            Type valueType,
            object value);

        new object this[Type valueType] { get; set; }

        void Update<TValue>(
            TValue value);

        void Update(
            Type valueType,
            object value);

        bool TryUpdate<TValue>(
            TValue value);

        bool TryUpdate(
            Type valueType,
            object value);

        void AddOrUpdate<TValue>(
            TValue value);

        void AddOrUpdate(
            Type valueType,
            object value);

        void Remove<TValue>();

        void Remove(
            Type valueType);

        bool TryRemove<TValue>();

        bool TryRemove(
            Type valueType);

        void Clear();
    }
}