using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta.Factories
{
	public class FloatTimerFactory
	{
		private readonly NonAllocBroadcasterFactory
			nonAllocBroadcasterFactory;

		private readonly RepositoryFactory repositoryFactory;

		private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

		public FloatTimerFactory(
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
			RepositoryFactory repositoryFactory,
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory)
		{
			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.repositoryFactory = repositoryFactory;

			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;
		}

		public FloatTimer BuildFloatTimer()
		{
			var context = BuildFloatTimerContext();

			var result = new FloatTimer(
				context,
				nonAllocSubscriptionFactory);

			var resultAsDependencyProvideable = context
				as ITimerDependencyProvideable<IFloatTimer>;

			if (resultAsDependencyProvideable != null)
			{
				resultAsDependencyProvideable.
					Timer = result;
			}

			return result;
		}

		public FloatTimerContext BuildFloatTimerContext()
		{
			return new FloatTimerContext(
				BuildFloatTimerDTO(),

				BuildStateRepository(),

				nonAllocBroadcasterFactory.
					BuildNonAllocBroadcasterGeneric<IFloatTimer>(),

				nonAllocBroadcasterFactory.
					BuildNonAllocBroadcasterGeneric<IFloatTimer>(),

				nonAllocBroadcasterFactory.
					BuildNonAllocBroadcasterGeneric<IFloatTimer>(),

				nonAllocBroadcasterFactory.
					BuildNonAllocBroadcasterGeneric<IFloatTimer>());
		}

		public FloatTimerDTO BuildFloatTimerDTO()
		{
			return new FloatTimerDTO
			{
				ID = TimerConsts.ANONYMOUS_TIMER_ID,
				State = ETimerState.INACTIVE,
				CurrentTimeElapsed = 0f,
				Accumulate = false,
				Repeat = false,
				FlushTimeElapsedOnRepeat = false,
				FireRepeatCallbackOnFinish = true,
				CurrentDuration = 0f,
				DefaultDuration = 0f
			};
		}

		/// Builds the repository of timer strategies for a runtime timer
		/// </summary>
		/// <returns>The built repository of timer strategies.</returns>
		private DictionaryRepository<ETimerState, IFloatTimerState> 
			BuildStateRepository()
		{
			var repository = repositoryFactory
				.BuildDictionaryRepository<ETimerState, IFloatTimerState>(
					new TimerStateComparer());

			repository.Add(
				ETimerState.INACTIVE,
				new FloatInactiveState());

			repository.Add(
				ETimerState.STARTED,
				new FloatStartedState());

			repository.Add(
				ETimerState.PAUSED,
				new FloatPausedState());

			repository.Add(
				ETimerState.FINISHED,
				new FloatFinishedState());

			return repository;
		}
	}
}