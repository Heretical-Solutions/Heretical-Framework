using System;

namespace HereticalSolutions.Allocations
{
    public class AllocationCommand<T>
        : IAllocationCommand<T>
    {
        private readonly AllocationCommandDescriptor descriptor;

        private readonly Func<T> allocationDelegate;

        private readonly IAllocationCallback<T> allocationCallback;

        public AllocationCommand(
            AllocationCommandDescriptor descriptor,
            Func<T> allocationDelegate,
            IAllocationCallback<T> allocationCallback)
        {
            this.descriptor = descriptor;
            this.allocationDelegate = allocationDelegate;
            this.allocationCallback = allocationCallback;
        }

        #region IAllocationCommand<T>

        public AllocationCommandDescriptor Descriptor => descriptor;

        public Func<T> AllocationDelegate => allocationDelegate;

        public IAllocationCallback<T> AllocationCallback => allocationCallback;

        #endregion
    }
}