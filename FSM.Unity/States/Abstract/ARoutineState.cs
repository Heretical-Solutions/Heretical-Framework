using System.Collections;

using UnityEngine;

namespace HereticalSolutions.FSM.Unity
{
    public abstract class ARoutineState
        : IState
    {
        protected readonly MonoBehaviour monoBehaviour;

        protected Coroutine currentRoutine;

        protected bool earlyReturned;
        
        public ARoutineState(
            MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
            
            earlyReturned = false;
        }

        #region IState

        public virtual void EnterState()
        {
            currentRoutine = monoBehaviour.StartCoroutine(
                Routine());

            if (earlyReturned)
            {
                if (currentRoutine != null)
                    monoBehaviour.StopCoroutine(
                        currentRoutine);
                
                earlyReturned = false;
                
                currentRoutine = null;
            }
        }

        public virtual void EnterState(
            ITransitionRequest transitionRequest)
        {
            EnterState();
        }

        public virtual void ExitState()
        {
            if (currentRoutine == null)
            {
                earlyReturned = true;
            }
            else
            {
                monoBehaviour.StopCoroutine(currentRoutine);
            }
        }

        public virtual void ExitState(
            ITransitionRequest transitionRequest)
        {
        }

        #endregion

        public abstract IEnumerator Routine();
    }
}