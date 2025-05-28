using System;
using System.Collections.Generic;

namespace HereticalSolutions.FSM.Builders
{
	public class StateMachineBuilderContext<TBaseState>
		where TBaseState : IState
	{
		public Dictionary<Type, TBaseState> States;

		public Dictionary<Type, ITransitionEvent<TBaseState>> Events;

		public ITransitionController<TBaseState> TransitionController;
	}
}