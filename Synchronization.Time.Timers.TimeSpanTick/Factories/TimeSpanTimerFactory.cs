using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick.Factories
{
	public class TimeSpanTimerFactory
	{
		private readonly NonAllocBroadcasterFactory
			nonAllocBroadcasterFactory;

		private readonly RepositoryFactory repositoryFactory;

		private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

		public TimeSpanTimerFactory(
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
			RepositoryFactory repositoryFactory,
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory)
		{
			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.repositoryFactory = repositoryFactory;

			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;
		}

		public TimeSpanTimer BuildTimeSpanTimer()
		{
			var context = BuildTimeSpanTimerContext();

			var result = new TimeSpanTimer(
				context,
				nonAllocSubscriptionFactory);

			var resultAsDependencyProvideable = context
				as ITimerDependencyProvideable<ITimeSpanTimer>;

			if (resultAsDependencyProvideable != null)
			{
				resultAsDependencyProvideable.
					Timer = result;
			}

			return result;
		}

		public TimeSpanTimerContext BuildTimeSpanTimerContext()
		{
			return new TimeSpanTimerContext(
				BuildTimeSpanTimerDTO(),

				BuildStateRepository(),

				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITimeSpanTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITimeSpanTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITimeSpanTimer>(),
				nonAllocBroadcasterFactory.

					BuildNonAllocBroadcasterGeneric<ITimeSpanTimer>());
		}

		public TimeSpanTimerDTO BuildTimeSpanTimerDTO()
		{
			return new TimeSpanTimerDTO
			{
				ID = TimerConsts.ANONYMOUS_TIMER_ID,
				State = ETimerState.INACTIVE,
				StartTime = default,
				EstimatedFinishTime = default,
				SavedProgress = default,
				Accumulate = false,
				Repeat = false,
				FlushTimeElapsedOnRepeat = false,
				FireRepeatCallbackOnFinish = true,
				CurrentDurationSpan = default,
				DefaultDurationSpan = default
			};
		}

		/// Builds the repository of timer strategies for a runtime timer
		/// </summary>
		/// <returns>The built repository of timer strategies.</returns>
		private DictionaryRepository<ETimerState, ITimeSpanTimerState>
			BuildStateRepository()
		{
			var repository = repositoryFactory
				.BuildDictionaryRepository<ETimerState, ITimeSpanTimerState>(
					new TimerStateComparer());

			repository.Add(
				ETimerState.INACTIVE,
				new TimeSpanInactiveState());

			repository.Add(
				ETimerState.STARTED,
				new TimeSpanStartedState());

			repository.Add(
				ETimerState.PAUSED,
				new TimeSpanPausedState());

			repository.Add(
				ETimerState.FINISHED,
				new TimeSpanFinishedState());

			return repository;
		}
	}
}