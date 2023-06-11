using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.StateMachines.Factories
{
    public static partial class StateMachineFactory
    {
        public static T AddState<TBaseState, T>(
            IRepository<Type, TBaseState> states)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T));
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }
        
        public static T AddStateWithArguments<TBaseState, T>(
            IRepository<Type, TBaseState> states,
            object[] arguments)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T),
                arguments);
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }
        
        private static TEvent AddTransitionEvent<TBaseState, TEvent, TFrom, TTo>(
            IRepository<Type, TBaseState> states,
            IRepository<Type, ITransitionEvent<TBaseState>> transitionEvents)
            where TBaseState : IState
            where TEvent : ITransitionEvent<TBaseState>
            where TFrom : TBaseState
            where TTo : TBaseState
        {
            var transitionEvent = (TEvent)Activator.CreateInstance(
                typeof(TEvent),
                new object[]
                {
                    states.Get(typeof(TFrom)),
                    states.Get(typeof(TTo)),
                });

            transitionEvents.Add(typeof(TEvent), transitionEvent);

            return transitionEvent;
        }
    }
}