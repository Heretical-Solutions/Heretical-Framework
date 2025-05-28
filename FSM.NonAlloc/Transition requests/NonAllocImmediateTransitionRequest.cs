using System;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc
{
	public class NonAllocImmediateTransitionRequest
		: ANonAllocTransitionRequest
	{
		private Type targetStateType;

		public Type TargetStateType
		{
			get => targetStateType;
			set => targetStateType = value;
		}

		public NonAllocImmediateTransitionRequest(
			INonAllocSubscribable onPreviousStateExited,
			INonAllocSubscribable onNextStateEntered,

			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
			: base(
				onPreviousStateExited,
				onNextStateEntered,

				previousStateExitProgress,
				nextStateEnterProgress)
		{
		}
	}
}