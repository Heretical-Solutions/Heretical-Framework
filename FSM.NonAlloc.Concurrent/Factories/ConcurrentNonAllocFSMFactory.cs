using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Repositories;

using HereticalSolutions.FSM.NonAlloc.Factories;

namespace HereticalSolutions.FSM.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocFSMFactory
	{
		private readonly NonAllocFSMFactory
			nonAllocFSMFactory;

		public ConcurrentNonAllocFSMFactory(
			NonAllocFSMFactory nonAllocFSMFactory)
		{
			this.nonAllocFSMFactory = nonAllocFSMFactory;
		}

		public ConcurrentBaseNonAllocStateMachine<TBaseState>
			BuildConcurrentBaseNonAllocStateMachine<TBaseState, TInitialState>(
				IRepository<Type, TBaseState> states,
				IRepository<Type, INonAllocTransitionEvent<TBaseState>> events,
				INonAllocTransitionController<TBaseState> transitionController)
				where TInitialState : TBaseState
				where TBaseState : INonAllocState
		{
			return new ConcurrentBaseNonAllocStateMachine<TBaseState>(
				nonAllocFSMFactory.BuildBaseNonAllocStateMachine<
					TBaseState,
					TInitialState>(
					states,
					events,
					transitionController),
				new object());
		}

		public ConcurrentBaseNonAllocStateMachine<TBaseState>
			BuildConcurrentBaseNonAllocStateMachine<TBaseState, TInitialState>(
				IRepository<Type, TBaseState> states,
				IRepository<Type, INonAllocTransitionEvent<TBaseState>> events,
				INonAllocTransitionController<TBaseState> transitionController,

				AllocationCommandDescriptor initial,
				AllocationCommandDescriptor additional)
				where TInitialState : TBaseState
				where TBaseState : INonAllocState
		{
			return new ConcurrentBaseNonAllocStateMachine<TBaseState>(
				nonAllocFSMFactory.BuildBaseNonAllocStateMachine<
					TBaseState,
					TInitialState>(
					states,
					events,
					transitionController,
					
					initial,
					additional),
				new object());
		}
	}
}