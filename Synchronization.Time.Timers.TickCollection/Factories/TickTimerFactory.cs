using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection.Factories
{
	public class TickTimerFactory
	{
		private readonly NonAllocBroadcasterFactory
			nonAllocBroadcasterFactory;

		private readonly RepositoryFactory repositoryFactory;

		private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

		public TickTimerFactory(
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
			RepositoryFactory repositoryFactory,
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory)
		{
			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.repositoryFactory = repositoryFactory;

			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;
		}

		public TickTimer BuildTickTimer()
		{
			var context = BuildTickTimerContext();

			var result = new TickTimer(
				context,
				nonAllocSubscriptionFactory);

			var resultAsDependencyProvideable = context
				as ITimerDependencyProvideable<ITickTimer>;

			if (resultAsDependencyProvideable != null)
			{
				resultAsDependencyProvideable.
					Timer = result;
			}

			return result;
		}

		public TickTimerContext BuildTickTimerContext()
		{
			return new TickTimerContext(
				BuildTickTimerDTO(),

				BuildStateRepository(),

				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITickTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITickTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITickTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITickTimer>());
		}

		public TickTimerDTO BuildTickTimerDTO()
		{
			return new TickTimerDTO
			{
				ID = TimerConsts.ANONYMOUS_TIMER_ID,
				State = ETimerState.INACTIVE,
				CurrentTimeElapsed = 0,
				Accumulate = false,
				Repeat = false,
				FlushTimeElapsedOnRepeat = false,
				FireRepeatCallbackOnFinish = true,
				CurrentDuration = 0,
				DefaultDuration = 0
			};
		}

		/// Builds the repository of timer strategies for a runtime timer
		/// </summary>
		/// <returns>The built repository of timer strategies.</returns>
		private DictionaryRepository<ETimerState, ITickTimerState>
			BuildStateRepository()
		{
			var repository = repositoryFactory
				.BuildDictionaryRepository<ETimerState, ITickTimerState>(
					new TimerStateComparer());

			repository.Add(
				ETimerState.INACTIVE,
				new TickInactiveState());

			repository.Add(
				ETimerState.STARTED,
				new TickStartedState());

			repository.Add(
				ETimerState.PAUSED,
				new TickPausedState());

			repository.Add(
				ETimerState.FINISHED,
				new TickFinishedState());

			return repository;
		}
	}
}