using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
    public interface IAsyncNonAllocTransitionController<TBaseState>
        where TBaseState : IAsyncNonAllocState
    {
        Task EnterState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task EnterState(
            TBaseState state,

            IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ExitState(
            TBaseState state,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ExitState(
            TBaseState state,

            IAsyncNonAllocTransitionRequest transitionRequest,

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}