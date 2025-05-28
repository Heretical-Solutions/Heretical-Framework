using HereticalSolutions.ObjectPools.Managed.Async.Builders;
using HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Builders
{
	public static class ConcurrentDecoratorAsyncManagedPoolBuilder
	{
		public static AsyncManagedPoolBuilder<T> DecoratedWithConcurrent<T>(
			this AsyncManagedPoolBuilder<T> builder,
			ConcurrentDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildConcurrentDecoratorAsyncManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static AsyncManagedPoolBuilder<T> DecoratedWithListConcurrent<T>(
			this AsyncManagedPoolBuilder<T> builder,
			ConcurrentDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildConcurrentDecoratorAsyncManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}