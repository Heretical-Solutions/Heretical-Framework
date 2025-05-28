using HereticalSolutions.ObjectPools.Managed.Async.Builders;
using HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Builders
{
	public static class CleanupDecoratorAsyncManagedPoolBuilder
	{
		public static AsyncManagedPoolBuilder<T> DecoratedWithCleanup<T>(
			this AsyncManagedPoolBuilder<T> builder,
			CleanupDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildCleanupDecoratorAsyncManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static AsyncManagedPoolBuilder<T> DecoratedWithListCleanup<T>(
			this AsyncManagedPoolBuilder<T> builder,
			CleanupDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildCleanupDecoratorAsyncManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}