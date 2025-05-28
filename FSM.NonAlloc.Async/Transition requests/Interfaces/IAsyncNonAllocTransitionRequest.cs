using System;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
	public interface IAsyncNonAllocTransitionRequest
	{
		public ETransitionState TransitionState { get; set; }

		public EAsyncTransitionRules Rules { get; set; }


		public bool CommencePreviousStateExitStart { get; set; }

		public INonAllocSubscribable OnPreviousStateExited { get; }

		public bool CommencePreviousStateExitFinish { get; set; }


		public bool CommenceNextStateEnterStart { get; set; }

		public INonAllocSubscribable OnNextStateEntered { get; }

		public bool CommenceNextStateEnterFinish { get; set; }


		public IProgress<float> PreviousStateExitProgress { get; set; }

		public IProgress<float> NextStateEnterProgress { get; set; }


		public AsyncExecutionContext AsyncContext { get; set; }
	}
}