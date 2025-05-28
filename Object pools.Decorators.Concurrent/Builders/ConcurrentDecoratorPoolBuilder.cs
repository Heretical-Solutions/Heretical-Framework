using HereticalSolutions.ObjectPools.Builders;
using HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Builders
{
	public static class ConcurrentDecoratorPoolBuilder
	{
		public static PoolBuilder<T> DecoratedWithConcurrent<T>(
			this PoolBuilder<T> builder,
			ConcurrentDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildConcurrentDecoratorPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static PoolBuilder<T> DecoratedWithListConcurrent<T>(
			this PoolBuilder<T> builder,
			ConcurrentDecoratorPoolFactory cleanupDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = cleanupDecoratorPoolFactory.
						BuildConcurrentDecoratorPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}