using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.FSM.NonAlloc.Async.Builders
{
	public class AsyncNonAllocStateMachineBuilderContext<TBaseState>
		where TBaseState : IAsyncNonAllocState
	{
		public IRepository<Type, TBaseState> States;

		public IRepository<Type, IAsyncNonAllocTransitionEvent<TBaseState>> Events;

		public IAsyncNonAllocTransitionController<TBaseState> TransitionController;
	}
}