namespace HereticalSolutions.FSM.NonAlloc
{
	public interface INonAllocTransitionController<TBaseState>
		where TBaseState : INonAllocState
	{
		void EnterState(
			TBaseState state);

		void EnterState(
			TBaseState state,
			INonAllocTransitionRequest transitionRequest);

		void ExitState(
			TBaseState state);

		void ExitState(
			TBaseState state,
			INonAllocTransitionRequest transitionRequest);
	}
}