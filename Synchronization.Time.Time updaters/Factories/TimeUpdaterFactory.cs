using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta.Factories;
using HereticalSolutions.Synchronization.Time.Timers.TickCollection.Factories;

namespace HereticalSolutions.Synchronization.Time.TimeUpdaters.Factories
{
	public class TimeUpdaterFactory
	{
		private const string ACCUMULATOR_ID = "Accumulator";

		private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

		private readonly NonAllocPingerFactory nonAllocPingerFactory;

		private readonly NonAllocBroadcasterFactory nonAllocBroadcasterFactory;

		private readonly FloatTimerFactory floatTimerFactory;

		private readonly TickTimerFactory tickTimerFactory;

		public TimeUpdaterFactory(
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory,
			NonAllocPingerFactory nonAllocPingerFactory,
			NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
			FloatTimerFactory floatTimerFactory,
			TickTimerFactory tickTimerFactory)
		{
			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;

			this.nonAllocPingerFactory = nonAllocPingerFactory;

			this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

			this.floatTimerFactory = floatTimerFactory;

			this.tickTimerFactory = tickTimerFactory;
		}

		public TimeUpdater BuildTimeUpdater(
			string id,
			bool togglable = true,
			bool active = true,
			bool scalable = true,
			float scale = 1f,
			bool hasFixedDelta = false,
			float fixedDelta = 0.02f,
			bool canHaveNegativeDelta = false)
		{
			var descriptor = new TimeUpdaterDescriptor(
				id,
				togglable,
				active,
				scalable,
				scale,
				hasFixedDelta,
				fixedDelta,
				canHaveNegativeDelta);

			var broadcaster = nonAllocBroadcasterFactory
				.BuildNonAllocBroadcasterGeneric<float>();

			var timer = floatTimerFactory
				.BuildFloatTimer();

			timer.Context.DTO.ID = ACCUMULATOR_ID;
			timer.Context.DTO.Accumulate = true;

			return new TimeUpdater(
				descriptor,
				broadcaster,
				nonAllocSubscriptionFactory,
				timer);
		}

		public TickUpdater BuildTickUpdater(
			string id,
			bool togglable = true,
			bool active = true)
		{
			var descriptor = new TickUpdaterDescriptor(
				id,
				togglable,
				active);

			var pinger = nonAllocPingerFactory
				.BuildNonAllocPinger();

			var timer = tickTimerFactory
				.BuildTickTimer();

			timer.Context.DTO.ID = ACCUMULATOR_ID;
			timer.Context.DTO.Accumulate = true;

			return new TickUpdater(
				descriptor,
				pinger,
				nonAllocSubscriptionFactory,
				timer);
		}
	}
}