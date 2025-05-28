/*
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.FSM.NonAlloc
{
    public class AnimationTransitionController<TBaseState> : IAsyncTransitionController<TBaseState>
        where TBaseState : IState
    {
        public async Task EnterState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            await ((IStateWithAnimatedTransition)state).StateEnterAnimationHandler.Play(state);
        }

        public async Task ExitState(
            TBaseState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            await ((IStateWithAnimatedTransition)state).StateExitAnimationHandler.Play(state);
        }
    }
}
*/