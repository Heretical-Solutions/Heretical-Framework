using System;

namespace HereticalSolutions.FSM
{
	public interface ITransitionRequest
	{
		ETransitionState TransitionState { get; set; }


		Action<IState> OnPreviousStateExited { get; }

		Action<IState> OnNextStateEntered { get; }


		IProgress<float> PreviousStateExitProgress { get; }

		IProgress<float> NextStateEnterProgress { get; }
	}
}