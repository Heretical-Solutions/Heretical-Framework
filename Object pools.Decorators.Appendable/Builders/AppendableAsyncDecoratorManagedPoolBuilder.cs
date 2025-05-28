using HereticalSolutions.ObjectPools.Managed.Async.Builders;
using HereticalSolutions.ObjectPools.Decorators.Appendable.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable.Builders
{
	public static class AppendableAsyncDecoratorManagedPoolBuilder
	{
		public static AsyncManagedPoolBuilder<T> DecoratedWithAppendable<T>(
			this AsyncManagedPoolBuilder<T> builder,
			AppendableDecoratorAsyncPoolFactory appendableDecoratorAsyncPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				async (delegateContext, asyncContext) =>
				{
					var decoratorPool = appendableDecoratorAsyncPoolFactory.
						BuildAppendableAsyncDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}