using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class AddressDecoratorsPoolsFactory
    {
        #region Decorator pools
        
        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithAddress<T> BuildNonAllocPoolWithAddress<T>(
            IRepository<int, INonAllocDecoratedPool<T>> repository,
            int level)
        {
            return new NonAllocPoolWithAddress<T>(repository, level);
        }
        
        #endregion
    }
}