using System;

namespace HereticalSolutions.FSM
{
	public class ImmediateTransitionRequest
		: ATransitionRequest
	{
		private readonly Type targetStateType;
		
		public Type TargetStateType => targetStateType;

		public ImmediateTransitionRequest(
			Type targetStateType,
			IProgress<float> previousStateExitProgress = null,
			IProgress<float> nextStateEnterProgress = null)
			: base (
				previousStateExitProgress,
				nextStateEnterProgress)
		{
			this.targetStateType = targetStateType;
		}

	}
}