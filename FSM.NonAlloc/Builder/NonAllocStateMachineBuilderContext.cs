using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.FSM.NonAlloc.Builders
{
	public class NonAllocStateMachineBuilderContext<TBaseState>
		where TBaseState : INonAllocState
	{
		public IRepository<Type, TBaseState> States;

		public IRepository<Type, INonAllocTransitionEvent<TBaseState>> Events;

		public INonAllocTransitionController<TBaseState> TransitionController;
	}
}