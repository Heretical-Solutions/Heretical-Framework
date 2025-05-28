using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Builders
{
	public static class CleanupDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithCleanup<T>(
			this ManagedPoolBuilder<T> builder,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildCleanupDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static ManagedPoolBuilder<T> DecoratedWithListCleanup<T>(
			this ManagedPoolBuilder<T> builder,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildCleanupDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}