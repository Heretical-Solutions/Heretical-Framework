using System;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
	public class AsyncNonAllocImmediateTransitionRequest
		: AAsyncNonAllocTransitionRequest
	{
		private Type targetStateType;

		public Type TargetStateType
		{
			get
			{
				lock (lockObject)
				{
					return targetStateType;
				}
			}
			set
			{
				lock (lockObject)
				{
					targetStateType = value;
				}
			}
		}

		public AsyncNonAllocImmediateTransitionRequest(
			object lockObject,

			INonAllocSubscribable onPreviousStateExited,
			INonAllocSubscribable onNextStateEntered,

			EAsyncTransitionRules rules = EAsyncTransitionRules.EXIT_THEN_ENTER,

			bool commencePreviousStateExitStart = true,
			bool commencePreviousStateExitFinish = true,

			bool commenceNextStateEnterStart = true,
			bool commenceNextStateEnterFinish = true,

			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
			: base(
				lockObject,

				onPreviousStateExited,
				onNextStateEntered,

				rules,

				commencePreviousStateExitStart,
				commencePreviousStateExitFinish,

				commenceNextStateEnterStart,
				commenceNextStateEnterFinish,
				
				previousStateExitProgress,
				nextStateEnterProgress)
		{
		}
	}
}