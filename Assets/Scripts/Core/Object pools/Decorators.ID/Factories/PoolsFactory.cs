using HereticalSolutions.Pools.Decorators;

namespace HereticalSolutions.Pools.Factories
{
    public static class IDDecoratorsPoolsFactory
    {
        #region Decorator pools

        public static PoolWithID<T> BuildPoolWithID<T>(
            IDecoratedPool<T> innerPool,
            string id)
        {
            return new PoolWithID<T>(innerPool, id);
        }

        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithID<T> BuildNonAllocPoolWithID<T>(
            INonAllocDecoratedPool<T> innerPool,
            string id)
        {
            return new NonAllocPoolWithID<T>(innerPool, id);
        }

        #endregion
    }
}