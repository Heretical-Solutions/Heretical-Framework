using System;

namespace HereticalSolutions.Persistence
{
    public interface ISerializer
        : IHasIODestination,
          IHasReadWriteControl
    {
        IReadOnlySerializerContext Context { get; }

        void EnsureMediumInitializedForDeserialization();

        void EnsureMediumFinalizedForDeserialization();

        void EnsureMediumInitializedForSerialization();

        void EnsureMediumFinalizedForSerialization();

        #region Serialize

        bool Serialize<TValue>(
            TValue value);
        
        bool Serialize(
            Type valueType,
            object valueObject);

        #endregion

        #region Deserialize

        bool Deserialize<TValue>(
            out TValue value);

        bool Deserialize(
            Type valueType,
            out object valueObject);

        #endregion

        #region Populate

        bool Populate<TValue>(
            TValue value);

        bool Populate(
            Type valueType,
            object valueObject);

        #endregion
    }
}