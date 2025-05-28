using System;

namespace HereticalSolutions.FSM
{
	public abstract class ATransitionRequest
		: ITransitionRequest
	{
		public ATransitionRequest(
			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
		{
			TransitionState = ETransitionState.UNINITIALISED;
			
			OnPreviousStateExited = null;
			OnNextStateEntered = null;
			
			PreviousStateExitProgress = previousStateExitProgress;
			NextStateEnterProgress = nextStateEnterProgress;
		}

		
		#region ITransitionRequest

		public ETransitionState TransitionState { get; set; }


		public Action<IState> OnPreviousStateExited { get; set; }

		public Action<IState> OnNextStateEntered { get; set; }

		public IProgress<float> PreviousStateExitProgress { get; set; }

		public IProgress<float> NextStateEnterProgress { get; set; }

		#endregion
	}
}