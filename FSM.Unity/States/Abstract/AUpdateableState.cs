namespace HereticalSolutions.FSM.Unity
{
    public abstract class AUpdateableState
        : IState
    {
        #region IState

        public virtual void EnterState()
        {
        }

        public virtual void EnterState(
            ITransitionRequest transitionRequest)
        {
            EnterState();
        }

        public virtual void ExitState()
        {
        }

        public virtual void ExitState(
            ITransitionRequest transitionRequest)
        {
        }

        #endregion

        public abstract void Update();
    }
}