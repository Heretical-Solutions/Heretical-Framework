using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
	public interface IAsyncNonAllocState
	{
		Task EnterState(
			
			//Async tail
			AsyncExecutionContext asyncContext);

		Task EnterState(
			IAsyncNonAllocTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task ExitState(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task ExitState(
			IAsyncNonAllocTransitionRequest transitionRequest,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}