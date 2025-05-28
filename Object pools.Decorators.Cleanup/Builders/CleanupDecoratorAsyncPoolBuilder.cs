using HereticalSolutions.ObjectPools.Async.Builders;
using HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Builders
{
	public static class CleanupDecoratorAsyncPoolBuilder
	{
		public static AsyncPoolBuilder<T> DecoratedWithCleanup<T>(
			this AsyncPoolBuilder<T> builder,
			CleanupDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildCleanupDecoratorAsyncPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static AsyncPoolBuilder<T> DecoratedWithListCleanup<T>(
			this AsyncPoolBuilder<T> builder,
			CleanupDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildCleanupDecoratorAsyncPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}