using System;
using System.Collections.Generic;

using HereticalSolutions.FSM.Builders;

namespace HereticalSolutions.FSM.Factories
{
    public class FSMFactory
    {
        public StateMachineBuilder<TBaseState>
            BuildStateMachineBuilder<TBaseState>()
            where TBaseState : IState
        {
            return new StateMachineBuilder<TBaseState>();
        }

        public BaseStateMachine<TBaseState>
            BuildBaseStateMachine<TBaseState, TInitialState>(
                Dictionary<Type, TBaseState> states,
                Dictionary<Type, ITransitionEvent<TBaseState>> events,
                ITransitionController<TBaseState> transitionController)
            where TInitialState : TBaseState
            where TBaseState : IState
        {
            return new BaseStateMachine<TBaseState>(
                states,
                events,

                transitionController,
                new Queue<ITransitionRequest>(),

                states[typeof(TInitialState)]);
        }
    }
}