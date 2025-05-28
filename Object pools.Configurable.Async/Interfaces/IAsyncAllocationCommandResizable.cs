using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools
{
	public interface IAsyncAllocationCommandResizable<T>
	{
		Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}