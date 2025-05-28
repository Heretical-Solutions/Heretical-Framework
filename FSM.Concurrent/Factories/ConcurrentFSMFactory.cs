using System;
using System.Collections.Generic;

using HereticalSolutions.FSM.Factories;

namespace HereticalSolutions.FSM.Concurrent.Factories
{
	public class ConcurrentFSMFactory
	{
		private readonly FSMFactory
			fsmFactory;

		public ConcurrentFSMFactory(
			FSMFactory fsmFactory)
		{
			this.fsmFactory = fsmFactory;
		}

		public ConcurrentBaseStateMachine<TBaseState>
			BuildConcurrentBaseStateMachine<TBaseState, TInitialState>(
				Dictionary<Type, TBaseState> states,
				Dictionary<Type, ITransitionEvent<TBaseState>> events,
				ITransitionController<TBaseState> transitionController)
				where TInitialState : TBaseState
				where TBaseState : IState
		{
			return new ConcurrentBaseStateMachine<TBaseState>(
				fsmFactory.BuildBaseStateMachine<
					TBaseState,
					TInitialState>(
					states,
					events,
					transitionController),
				new object());
		}
	}
}