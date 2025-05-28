using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public interface IAsyncPublisherSingleArgGeneric<TValue>
	{
		Task PublishAsync(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}