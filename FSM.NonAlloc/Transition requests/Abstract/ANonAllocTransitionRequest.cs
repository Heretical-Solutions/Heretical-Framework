using System;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc
{
	public abstract class ANonAllocTransitionRequest
		: INonAllocTransitionRequest
	{
		protected ETransitionState transitionState = ETransitionState.UNINITIALISED;


		protected INonAllocSubscribable onPreviousStateExited;

		protected INonAllocSubscribable onNextStateEntered;


		protected IProgress<float> previousStateExitProgress;

		protected IProgress<float> nextStateEnterProgress;

		public ANonAllocTransitionRequest(
			INonAllocSubscribable onPreviousStateExited,
			INonAllocSubscribable onNextStateEntered,

			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
		{
			transitionState = ETransitionState.UNINITIALISED;

			this.onPreviousStateExited = onPreviousStateExited;
			this.onNextStateEntered = onNextStateEntered;

			this.previousStateExitProgress = previousStateExitProgress;
			this.nextStateEnterProgress = nextStateEnterProgress;
		}

		#region INonAllocTransitionRequest

		public ETransitionState TransitionState
		{
			get => transitionState;
			set => transitionState = value;
		}


		public INonAllocSubscribable OnPreviousStateExited
		{
			get => onPreviousStateExited;
		}

		public INonAllocSubscribable OnNextStateEntered
		{
			get => onNextStateEntered;
		}


		public IProgress<float> PreviousStateExitProgress
		{
			get => previousStateExitProgress;
			set => previousStateExitProgress = value;
		}

		public IProgress<float> NextStateEnterProgress
		{
			get => nextStateEnterProgress;
			set => nextStateEnterProgress = value;
		}

		#endregion
	}
}