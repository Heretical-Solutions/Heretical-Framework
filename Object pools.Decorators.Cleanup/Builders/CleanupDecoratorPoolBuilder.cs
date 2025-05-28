using HereticalSolutions.ObjectPools.Builders;
using HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Builders
{
	public static class CleanupDecoratorPoolBuilder
	{
		public static PoolBuilder<T> DecoratedWithCleanup<T>(
			this PoolBuilder<T> builder,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildCleanupDecoratorPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static PoolBuilder<T> DecoratedWithListCleanup<T>(
			this PoolBuilder<T> builder,
			CleanupDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildCleanupDecoratorPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}