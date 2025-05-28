using HereticalSolutions.FSM.NonAlloc.Builders;

using HereticalSolutions.FSM.NonAlloc.Concurrent.Factories;

namespace HereticalSolutions.FSM.NonAlloc.Concurrent.Builders
{
	public static class ConcurrentNonAllocStateMachineBuilder
	{
		public static ConcurrentBaseNonAllocStateMachine<TBaseState>
			BuildConcurrentBaseNonAllocStateMachine<TBaseState, TInitialState>(
				this NonAllocStateMachineBuilder<TBaseState> builder,
				ConcurrentNonAllocFSMFactory concurrentNonAllocFSMFactory)
				where TInitialState : TBaseState
				where TBaseState : INonAllocState
		{
			var context = builder.Context;

			var result = concurrentNonAllocFSMFactory
				.BuildConcurrentBaseNonAllocStateMachine<TBaseState, TInitialState>(
					context.States,
					context.Events,
					context.TransitionController);

			builder.Cleanup();

			return result;
		}
	}
}