using HereticalSolutions.FSM.Builders;

using HereticalSolutions.FSM.Concurrent.Factories;

namespace HereticalSolutions.FSM.Concurrent.Builders
{
	public static class ConcurrentStateMachineBuilder
	{
		public static ConcurrentBaseStateMachine<TBaseState>
			BuildConcurrentBaseStateMachine<TBaseState, TInitialState>(
				this StateMachineBuilder<TBaseState> builder,
				ConcurrentFSMFactory concurrentFSMFactory)
				where TInitialState : TBaseState
				where TBaseState : IState
		{
			var context = builder.Context;

			var result = concurrentFSMFactory
				.BuildConcurrentBaseStateMachine<TBaseState, TInitialState>(
					context.States,
					context.Events,
					context.TransitionController);

			builder.Cleanup();

			return result;
		}
	}
}