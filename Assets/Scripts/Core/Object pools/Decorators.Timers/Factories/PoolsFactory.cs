using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Decorator pools
        
        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithRuntimeTimer<T> BuildNonAllocPoolWithRuntimeTimer<T>(
            INonAllocDecoratedPool<T> innerPool,
            ISynchronizationProvider synchronizationProvider)
        {
            return new NonAllocPoolWithRuntimeTimer<T>(innerPool, synchronizationProvider);
        }
        
        #endregion
    }
}