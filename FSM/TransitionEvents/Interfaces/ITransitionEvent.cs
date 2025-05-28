using System;

namespace HereticalSolutions.FSM
{
    public interface ITransitionEvent<TBaseState>
        where TBaseState : IState
    {
        Type From { get; }
        
        Type To { get; }
    }
}