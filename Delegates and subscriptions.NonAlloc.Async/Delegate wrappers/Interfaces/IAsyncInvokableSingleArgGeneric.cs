using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public interface IAsyncInvokableSingleArgGeneric<TValue>
	{
		Task InvokeAsync(
			TValue arg,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task InvokeAsync(
			object arg,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}