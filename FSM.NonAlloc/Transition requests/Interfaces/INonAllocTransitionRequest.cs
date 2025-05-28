using System;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc
{
	public interface INonAllocTransitionRequest
	{
		ETransitionState TransitionState { get; set; }


		INonAllocSubscribable OnPreviousStateExited { get; }

		INonAllocSubscribable OnNextStateEntered { get; }


		IProgress<float> PreviousStateExitProgress { get; set; }

		IProgress<float> NextStateEnterProgress { get; set; }
	}
}