using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public interface IAsyncPublisherNoArgs
	{
		Task PublishAsync(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}