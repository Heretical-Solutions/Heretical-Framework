using System;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
    public class ManagedObjectPoolAllocationCommandFactory
    {
        public AllocationCommand<IPoolElementFacade<T>>
            BuildPoolElementFacadeAllocationCommand<T>(
                AllocationCommandDescriptor descriptor,
                Func<IPoolElementFacade<T>> poolElementAllocationDelegate,
                IAllocationCallback<IPoolElementFacade<T>>
                    facadeAllocationCallback = null)
        {
            return new AllocationCommand<IPoolElementFacade<T>>(
                descriptor,
                poolElementAllocationDelegate,
                facadeAllocationCallback);
        }
    }
}