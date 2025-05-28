using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Appendable.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable.Builders
{
	public static class AppendableDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithAppendable<T>(
			this ManagedPoolBuilder<T> builder,
			AppendableDecoratorPoolFactory appendableDecoratorPoolFactory)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					var decoratorPool = appendableDecoratorPoolFactory.
						BuildAppendableDecoratorManagedPool(
							delegateContext.CurrentPool);

					delegateContext.CurrentPool = decoratorPool;
				});

			return builder;
		}
	}
}