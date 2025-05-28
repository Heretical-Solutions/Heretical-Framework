using System;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Synchronization.Time.TimerManagers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Timers
{
	public class TimerDecoratorManagedPool<T>
		: ADecoratorManagedPool<T>
	{
		private readonly ITimerManager timerManager;

		private readonly float defaultDuration;

		private readonly ILogger logger;
		
		public TimerDecoratorManagedPool(
			IManagedPool<T> innerPool,
			ITimerManager timerManager,
			float defaultDuration,
			ILogger logger)
			: base(
				innerPool)
		{
			this.timerManager = timerManager;

			this.defaultDuration = defaultDuration;

			this.logger = logger;
		}
		
		protected override void OnAfterPop(
			IPoolElementFacade<T> instance,
			IPoolPopArgument[] args)
		{
			IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

			if (instanceWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}
			
			if (!instanceWithMetadata.Metadata.Has<IContainsAllocatedTimer>())
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO TIMER METADATA"));

			//Get metadata

			var metadata = instanceWithMetadata.Metadata.Get<IContainsAllocatedTimer>();

			var metadataCasted = metadata as AllocatedTimerMetadata;

			//Calculate duration

			float duration = defaultDuration;

			if (args.TryGetArgument<DurationArgument>(out var arg))
			{
				duration = arg.Duration;
			}

			//Early return

			if (duration < 0f)
				return;


			//Get the timer

			if (!timerManager.TryAllocateTimer(
				out var allocatedTimerContext))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"FAILED TO ALLOCATE TIMER"));
			}

			allocatedTimerContext.Timer.Reset(
				duration);

			metadataCasted.TimerContext = allocatedTimerContext;

			allocatedTimerContext.Timer.OnFinish.Subscribe(
				metadataCasted.PushToPoolOnTimeoutSubscription);

			allocatedTimerContext.Timer.Start();
		}

		protected override void OnBeforePush(IPoolElementFacade<T> instance)
		{
			IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

			if (instanceWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}
			
			if (!instanceWithMetadata.Metadata.Has<IContainsAllocatedTimer>())
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO TIMER METADATA"));

			var metadata = instanceWithMetadata.Metadata.Get<IContainsAllocatedTimer>();

			var metadataCasted = metadata as AllocatedTimerMetadata;


			var allocatedTimerContext = metadata.TimerContext;

			if (allocatedTimerContext != null)
			{
				allocatedTimerContext.Timer.Reset(
					defaultDuration);

				if (metadataCasted
					.PushToPoolOnTimeoutSubscription
					.Active)
				{
					allocatedTimerContext.Timer.OnFinish.Unsubscribe(
						metadataCasted.PushToPoolOnTimeoutSubscription);
				}

				timerManager
					.TryFreeTimer(
						allocatedTimerContext);

				metadataCasted.TimerContext = null;
			}
		}
	}
}