using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StateMachines
{
    public interface ITransitionController<TBaseState>
        where TBaseState : IState
    {
        Task EnterState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);

        Task ExitState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);
    }
}