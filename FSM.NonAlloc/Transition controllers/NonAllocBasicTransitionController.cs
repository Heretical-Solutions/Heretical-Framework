using HereticalSolutions.Delegates;

namespace HereticalSolutions.FSM.NonAlloc
{
	public class NonAllocBasicTransitionController<TBaseState>
		: INonAllocTransitionController<TBaseState>
		where TBaseState : INonAllocState
	{
		#region ITransitionController

		public void EnterState(
			TBaseState state)
		{
			state.EnterState();
		}

		public void EnterState(
			TBaseState state,
			INonAllocTransitionRequest transitionRequest)
		{
			transitionRequest.NextStateEnterProgress?.Report(0f);

			state.EnterState(
				transitionRequest);

			transitionRequest.NextStateEnterProgress?.Report(1f);

			var publisher = transitionRequest.OnNextStateEntered
				as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		public void ExitState(
			TBaseState state)
		{
			state.ExitState();
		}

		public void ExitState(
			TBaseState state,
			INonAllocTransitionRequest transitionRequest)
		{
			transitionRequest.PreviousStateExitProgress?.Report(0f);

			state.ExitState(
				transitionRequest);

			transitionRequest.PreviousStateExitProgress?.Report(1f);

			var publisher = transitionRequest.OnPreviousStateExited
				as IPublisherSingleArgGeneric<TBaseState>;

			publisher?.Publish(
				state);
		}

		#endregion
	}
}