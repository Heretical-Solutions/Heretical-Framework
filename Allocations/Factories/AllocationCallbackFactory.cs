using System.Collections.Generic;

namespace HereticalSolutions.Allocations.Factories
{
    public class AllocationCallbackFactory
    {
        public CompositeAllocationCallback<T> BuildCompositeCallback<T>(
            IAllocationCallback<T>[] callbacks)
        {
            List<IAllocationCallback<T>> callbacksList =
                new List<IAllocationCallback<T>>(callbacks);
            
            return new CompositeAllocationCallback<T>(
                callbacksList);
        }

        public CompositeAllocationCallback<T> BuildCompositeCallback<T>(
            List<IAllocationCallback<T>> callbacks)
        {
            return new CompositeAllocationCallback<T>(
                callbacks);
        }
    }
}