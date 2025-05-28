using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Allocations.Async
{
	public interface IAsyncAllocationCallback<T>
	{
		Task OnAllocated(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}