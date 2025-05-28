using System;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
	public class AsyncNonAllocEventTransitionRequest
		: AAsyncNonAllocTransitionRequest
	{
		private Type eventType;

		public Type EventType
		{
			get
			{
				lock (lockObject)
				{
					return eventType;
				}
			}
			set
			{
				lock (lockObject)
				{
					eventType = value;
				}
			}
		}

		public AsyncNonAllocEventTransitionRequest(
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