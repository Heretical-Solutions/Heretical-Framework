namespace HereticalSolutions.FSM
{
    public interface IState
    {
        void EnterState();

        void EnterState(
            ITransitionRequest transitionRequest);
        
        void ExitState();

        void ExitState(
            ITransitionRequest transitionRequest);
    }
}