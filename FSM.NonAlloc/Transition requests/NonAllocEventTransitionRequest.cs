using System;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc
{
	public class NonAllocEventTransitionRequest
		: ANonAllocTransitionRequest
	{
		private Type eventType;

		public Type EventType
		{
			get => eventType;
			set => eventType = value;
		}

		public NonAllocEventTransitionRequest(
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