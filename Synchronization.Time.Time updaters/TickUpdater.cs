using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;
using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Synchronization.Time.Timers.TickCollection;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
	public class TickUpdater
		: ITickUpdater,
		  ICleanuppable,
		  IDisposable
	{
		private readonly TickUpdaterDescriptor descriptor;

		private readonly IPublisherNoArgs pinger;

		private readonly INonAllocSubscription synchronizationSubscription;

		private readonly ITickTimer accumulator;

		private readonly ILogger logger;

		public TickUpdater(
			TickUpdaterDescriptor descriptor,
			IPublisherNoArgs pinger,
			NonAllocSubscriptionFactory subscriptionFactory,
			ITickTimer accumulator)
		{
			this.descriptor = descriptor;

			this.pinger = pinger;

			this.accumulator = accumulator;

			synchronizationSubscription = subscriptionFactory
				.BuildSubscriptionNoArgs(
					Synchronize);


			accumulator?.Reset();

			accumulator?.Start();
		}

		#region ITimeUpdater

		#region ISynchronizationProvider

		public void Subscribe(
			ISynchronizable synchronizable)
		{
			var subscriber = synchronizable as ISynchronizationSubscriber;

			if (subscriber == null)
			{
				throw new ArgumentException(
					logger.TryFormatException(
						GetType(),
						"SYNCHRONIZABLE IS NOT SYNCHORNIZATION SUBSCRIBER"));
			}

			var subscribable = pinger as INonAllocSubscribable;

			subscribable.Subscribe(
				subscriber.SynchronizationSubscription);
		}

		public void Unsubscribe(
			ISynchronizable synchronizable)
		{
			var subscriber = synchronizable as ISynchronizationSubscriber;

			if (subscriber == null)
			{
				throw new ArgumentException(
					logger.TryFormatException(
						GetType(),
						"SYNCHRONIZABLE IS NOT SYNCHORNIZATION SUBSCRIBER"));
			}

			var subscribable = pinger as INonAllocSubscribable;

			subscribable.Unsubscribe(
				subscriber.SynchronizationSubscription);
		}

		public void UnsubscribeAll()
		{
			var subscribable = pinger as INonAllocSubscribable;

			subscribable.UnsubscribeAll();
		}

		#endregion

		#region ISynchronizable

		public void Synchronize()
		{
			if (descriptor.Togglable)
			{
				if (!descriptor.Active)
				{
					return;
				}
			}


			var accumulatorAsSynchronizable = accumulator
				as ISynchronizable;

			//Updater may as well have no accumulator
			accumulatorAsSynchronizable?.Synchronize();


			pinger.Publish();
		}

		#endregion

		#region ISynchronizationSubscriber

		public INonAllocSubscription SynchronizationSubscription =>
			synchronizationSubscription;

		#endregion

		public TickUpdaterDescriptor Descriptor => descriptor;

		public ITickTimer Accumulator => accumulator;

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (pinger is ICleanuppable)
				(pinger as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (pinger is IDisposable)
				(pinger as IDisposable).Dispose();
		}

		#endregion
	}
}