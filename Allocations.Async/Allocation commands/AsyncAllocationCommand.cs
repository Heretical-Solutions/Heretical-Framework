using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Allocations.Async
{
	public class AsyncAllocationCommand<T>
		: IAsyncAllocationCommand<T>
	{
		private readonly AllocationCommandDescriptor descriptor;

		private readonly Func<Task<T>> allocationDelegate;

		private readonly IAsyncAllocationCallback<T> allocationCallback;

		public AsyncAllocationCommand(
			AllocationCommandDescriptor descriptor,
			Func<Task<T>> allocationDelegate,
			IAsyncAllocationCallback<T> allocationCallback)
		{
			this.descriptor = descriptor;
			this.allocationDelegate = allocationDelegate;
			this.allocationCallback = allocationCallback;
		}

		#region IAsyncAllocationCommand<T>

		public AllocationCommandDescriptor Descriptor => descriptor;

		public Func<Task<T>> AllocationDelegate => allocationDelegate;

		public IAsyncAllocationCallback<T> AllocationCallback => allocationCallback;

		#endregion
	}
}