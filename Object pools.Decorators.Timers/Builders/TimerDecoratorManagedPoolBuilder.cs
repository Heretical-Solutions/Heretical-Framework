using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Timers.Factories;

using HereticalSolutions.Synchronization.Time.TimerManagers;

namespace HereticalSolutions.ObjectPools.Decorators.Timers.Builders
{
	public static class TimerDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithTimers<T>(
			this ManagedPoolBuilder<T> builder,
			ITimerManager timerManager,
			float duration,

			TimerDecoratorPoolFactory timerDecoratorPoolFactory,
			TimerDecoratorAllocationCallbackFactory
				timerDecoratorAllocationCallbackFactory,
			TimerDecoratorMetadataFactory
				timerDecoratorMetadataFactory)
		{
			var context = builder.Context;

			context
				.MetadataDescriptorBuilders
				.Add(
					timerDecoratorMetadataFactory
						.BuildAllocatedTimerMetadataMetadataDescriptor);

			SetPushToPoolOnTimeoutSubscriptionCallback<T>
				setPushToPoolOnTimeoutSubscriptionCallback =
					timerDecoratorAllocationCallbackFactory.
						BuildSetPushToPoolOnTimeoutSubscriptionCallback<T>();

			context.FacadeAllocationCallbacks.Add(
				setPushToPoolOnTimeoutSubscriptionCallback);

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = timerDecoratorPoolFactory.
						BuildTimerDecoratorManagedPool(
							delegateContext.CurrentPool,
							timerManager,
							duration);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}