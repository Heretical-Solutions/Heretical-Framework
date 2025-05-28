using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Allocations.Async
{
	public interface IAsyncAllocationCommand<T>
	{
		public AllocationCommandDescriptor Descriptor { get; }

		public Func<Task<T>> AllocationDelegate { get; }

		public IAsyncAllocationCallback<T> AllocationCallback { get; }
	}
}