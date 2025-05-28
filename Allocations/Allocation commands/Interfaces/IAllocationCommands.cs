using System;

namespace HereticalSolutions.Allocations
{
	public interface IAllocationCommand<T>
	{
		public AllocationCommandDescriptor Descriptor { get; }

		public Func<T> AllocationDelegate { get; }

		public IAllocationCallback<T> AllocationCallback { get; }
	}
}