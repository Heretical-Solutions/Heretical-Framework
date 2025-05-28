using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Builders
{
	public static class ConcurrentDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithConcurrent<T>(
			this ManagedPoolBuilder<T> builder,
			ConcurrentDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildConcurrentDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static ManagedPoolBuilder<T> DecoratedWithListConcurrent<T>(
			this ManagedPoolBuilder<T> builder,
			ConcurrentDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildConcurrentDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}