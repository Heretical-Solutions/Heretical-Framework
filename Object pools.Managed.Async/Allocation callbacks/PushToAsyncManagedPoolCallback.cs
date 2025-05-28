using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class PushToAsyncManagedPoolCallback<T>
		: IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
	{
		public IAsyncManagedPool<T> TargetPool { get; set; }

		public async Task OnAllocated(
			IAsyncPoolElementFacade<T> currentElementFacade,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			currentElementFacade.Pool = TargetPool;

			await TargetPool?.Push(
				currentElementFacade,
				asyncContext);
		}
	}
}