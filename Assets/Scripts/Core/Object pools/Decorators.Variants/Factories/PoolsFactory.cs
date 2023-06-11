using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.RandomGeneration;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class VariantsDecoratorsPoolsFactory
    {
        #region Decorator pools

        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithVariants<T> BuildNonAllocPoolWithVariants<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator)
        {
            return new NonAllocPoolWithVariants<T>(repository, generator);
        }

        #endregion
    }
}