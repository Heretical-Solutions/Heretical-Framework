using System;
using System.Threading.Tasks;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncManagedObjectPoolAllocationCommandFactory
	{
		public AsyncAllocationCommand<IAsyncPoolElementFacade<T>>
			BuildAsyncPoolElementFacadeAllocationCommand<T>(
				AllocationCommandDescriptor descriptor,
				Func<Task<IAsyncPoolElementFacade<T>>> poolElementAllocationDelegate,
				IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
					facadeAllocationCallback = null)
		{
			return new AsyncAllocationCommand<IAsyncPoolElementFacade<T>>(
				descriptor,
				poolElementAllocationDelegate,
				facadeAllocationCallback);
		}
	}
}