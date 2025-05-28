using HereticalSolutions.ObjectPools.Async.Builders;
using HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Builders
{
	public static class ConcurrentDecoratorAsyncPoolBuilder
	{
		public static AsyncPoolBuilder<T> DecoratedWithConcurrent<T>(
			this AsyncPoolBuilder<T> builder,
			ConcurrentDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildConcurrentDecoratorAsyncPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}

		public static AsyncPoolBuilder<T> DecoratedWithListConcurrent<T>(
			this AsyncPoolBuilder<T> builder,
			ConcurrentDecoratorAsyncPoolFactory cleanupDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = cleanupDecoratorAsyncPoolFactory.
						BuildConcurrentDecoratorAsyncPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}