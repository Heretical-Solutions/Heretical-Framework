using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public interface IAsyncManagedResizable<T>
	{
		Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}