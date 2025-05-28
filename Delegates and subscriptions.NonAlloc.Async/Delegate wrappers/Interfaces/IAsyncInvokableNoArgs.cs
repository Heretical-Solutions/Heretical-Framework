using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public interface IAsyncInvokableNoArgs
	{
		Task InvokeAsync(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}