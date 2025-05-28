namespace HereticalSolutions.FSM.NonAlloc
{
	public interface INonAllocState
	{
		void EnterState();

		void EnterState(
			INonAllocTransitionRequest transitionRequest);

		void ExitState();

		void ExitState(
			INonAllocTransitionRequest transitionRequest);
	}
}