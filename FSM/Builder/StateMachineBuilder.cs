using System;
using System.Collections.Generic;

using HereticalSolutions.Builders;

using HereticalSolutions.FSM.Factories;

namespace HereticalSolutions.FSM.Builders
{
    public class StateMachineBuilder<TBaseState>
        : ABuilder<StateMachineBuilderContext<TBaseState>>
        where TBaseState : IState
    {
        public StateMachineBuilder<TBaseState> New()
        {
            context = new StateMachineBuilderContext<TBaseState>
            {
                States = new Dictionary<Type, TBaseState>(),
                Events = new Dictionary<Type, ITransitionEvent<TBaseState>>(),
                TransitionController = new BasicTransitionController<TBaseState>()
            };

            return this;
        }

        public StateMachineBuilder<TBaseState> AddState<T>(
            T state)
            where T : TBaseState
        {
            context.States.Add(
                typeof(T),
                state);

            return this;
        }
        
        public T GetState<T>()
            where T : TBaseState
        {
            return (T)context.States[typeof(T)];
        }

        public Dictionary<Type, TBaseState> States =>
            context.States;

        public StateMachineBuilder<TBaseState> AddTransitionEvent<TEvent>(
            TEvent transitionEvent)
            where TEvent : ITransitionEvent<TBaseState>
        {
            context.Events.Add(
                typeof(TEvent),
                transitionEvent);

            return this;
        }

        public Dictionary<Type, ITransitionEvent<TBaseState>> Events =>
            context.Events;

        public BaseStateMachine<TBaseState> BuildBaseStateMachine<TInitialState>(
            FSMFactory fsmFactory)
            where TInitialState : TBaseState
        {
            var result =  fsmFactory.BuildBaseStateMachine<TBaseState, TInitialState>(
                context.States,
                context.Events,
                context.TransitionController);

            Cleanup();

            return result;
        }
    }
}