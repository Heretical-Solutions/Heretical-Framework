using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public interface IAsyncInvokableMultipleArgs
	{
		Task InvokeAsync(
			object[] args,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}