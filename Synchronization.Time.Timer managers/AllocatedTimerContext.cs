using HereticalSolutions.Delegates.NonAlloc;
using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;

namespace HereticalSolutions.Synchronization.Time.TimerManagers
{
	public class AllocatedTimerContext
	{
		public IFloatTimer Timer { get; private set; }

		public ITimerManager TimerManager { get; private set; }

		public INonAllocSubscription AttachToTimeUpdaterOnStartSubscription
		{ get; private set; }

		public INonAllocSubscription DetachFromTimeUpdaterOnStopSubscription
		{ get; private set; }

		public AllocatedTimerContext(
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory)
		{
			Timer = null;

			TimerManager = null;

			AttachToTimeUpdaterOnStartSubscription = nonAllocSubscriptionFactory
				.BuildSubscriptionSingleArgGeneric<IFloatTimer>(
					AttachToTimeUpdaterOnStart);

			DetachFromTimeUpdaterOnStopSubscription = nonAllocSubscriptionFactory
				.BuildSubscriptionSingleArgGeneric<IFloatTimer>(
					DetachFromTimeUpdaterOnStop);
		}

		private void AttachToTimeUpdaterOnStart(
			IFloatTimer timer)
		{
			var synchronizable = timer as ISynchronizableWithDelta<float>;

			TimerManager.TimeUpdater.Subscribe(synchronizable);
		}

		private void DetachFromTimeUpdaterOnStop(
			IFloatTimer timer)
		{
			var synchronizable = timer as ISynchronizableWithDelta<float>;

			TimerManager.TimeUpdater.Unsubscribe(synchronizable);
		}

		public void Initialize(
			IFloatTimer timer,
			ITimerManager timerManager)
		{
			Timer = timer;

			TimerManager = timerManager;

			Timer.OnStart.Subscribe(
				AttachToTimeUpdaterOnStartSubscription);

			Timer.OnFinish.Subscribe(
				DetachFromTimeUpdaterOnStopSubscription);
		}

		public void Cleanup()
		{
			if (Timer != null)
			{
				if (AttachToTimeUpdaterOnStartSubscription.Active)
					Timer.OnStart.Unsubscribe(
						AttachToTimeUpdaterOnStartSubscription);
	
				if (DetachFromTimeUpdaterOnStopSubscription.Active)
					Timer.OnFinish.Unsubscribe(
						DetachFromTimeUpdaterOnStopSubscription);
			}
			else
			{
				if (AttachToTimeUpdaterOnStartSubscription.Active)
					AttachToTimeUpdaterOnStartSubscription.Unsubscribe();

				if (DetachFromTimeUpdaterOnStopSubscription.Active)
					DetachFromTimeUpdaterOnStopSubscription.Unsubscribe();
			}

			Timer = null;

			TimerManager = null;
		}
	}
}