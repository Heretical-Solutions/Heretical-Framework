using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
    public class ManagedObjectPoolAllocationCallbackFactory
    {
        public PushToManagedPoolCallback<T>
            BuildPushToManagedPoolCallback<T>(
                IManagedPool<T> root = null)
        {
            return new PushToManagedPoolCallback<T>
            {
                TargetPool = root
            };
        }
        
        public PushToManagedPoolWhenAvailableCallback<T>
            BuildPushToManagedPoolWhenAvailableCallback<T>(
                IManagedPool<T> root = null)
        {
            return new PushToManagedPoolWhenAvailableCallback<T>(
                new List<IPoolElementFacade<T>>(),
                root);
        }
    }
}