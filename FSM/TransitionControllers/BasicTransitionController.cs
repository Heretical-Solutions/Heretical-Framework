namespace HereticalSolutions.FSM
{
	public class BasicTransitionController<TBaseState>
		: ITransitionController<TBaseState>
		where TBaseState : IState
	{
		#region ITransitionController

		public void EnterState(
			TBaseState state)
		{
			state.EnterState();
		}

		public void EnterState(
			TBaseState state,
			ITransitionRequest transitionRequest)
		{
			transitionRequest.NextStateEnterProgress?.Report(0f);

			state.EnterState(
				transitionRequest);

			transitionRequest.NextStateEnterProgress?.Report(1f);

			transitionRequest.OnNextStateEntered?.Invoke(
				state);
		}

		public void ExitState(
			TBaseState state)
		{
			state.ExitState();
		}

		public void ExitState(
			TBaseState state,
			ITransitionRequest transitionRequest)
		{
			transitionRequest.PreviousStateExitProgress?.Report(0f);

			state.ExitState(
				transitionRequest);

			transitionRequest.PreviousStateExitProgress?.Report(1f);

			transitionRequest.OnPreviousStateExited?.Invoke(
				state);
		}

		#endregion
	}
}