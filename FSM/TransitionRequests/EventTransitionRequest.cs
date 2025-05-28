using System;

namespace HereticalSolutions.FSM
{
	public class EventTransitionRequest
		: ATransitionRequest
	{
		private readonly Type eventType;

		public Type EventType => eventType;

		public EventTransitionRequest(
			Type eventType,
			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
			: base (
				previousStateExitProgress,
				nextStateEnterProgress)
		{
			this.eventType = eventType;
		}

	}
}