using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public interface IAsyncManagedPoolInternal<T>
	{
		IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
			FacadeAllocationCommand { get; }

		IAsyncAllocationCommand<T> ValueAllocationCommand { get; }

		Task<IAsyncPoolElementFacade<T>> PopFacade(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task PushFacade(
			IAsyncPoolElementFacade<T> facade,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}