using HereticalSolutions.ObjectPools.Builders;
using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.ObjectPools.Configurable.Builders
{
	public static class ConfigurablePoolBuilder
	{
		public static PoolBuilder<T> ConfigurableStackPool<T>(
			this PoolBuilder<T> builder,
			ConfigurableStackPoolFactory configurableStackPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						configurableStackPoolFactory.BuildConfigurableStackPool(
							delegateContext.InitialValueAllocationCommand,
							delegateContext.AdditionalValueAllocationCommand);
				};

			return builder;
		}

		public static PoolBuilder<T> ConfigurableQueuePool<T>(
			this PoolBuilder<T> builder,
			ConfigurableQueuePoolFactory configurableQueuePoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						configurableQueuePoolFactory.BuildConfigurableQueuePool(
							delegateContext.InitialValueAllocationCommand,
							delegateContext.AdditionalValueAllocationCommand);
				};

			return builder;
		}

		public static PoolBuilder<T> ConfigurablePackedArrayPool<T>(
			this PoolBuilder<T> builder,
			ConfigurablePackedArrayPoolFactory configurablePackedArrayPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						configurablePackedArrayPoolFactory.
							BuildConfigurablePackedArrayPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand);
				};

			return builder;
		}

		public static PoolBuilder<T> ConfigurableLinkedListPool<T>(
			this PoolBuilder<T> builder,
			ConfigurableLinkedListPoolFactory configurableLinkedListPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						configurableLinkedListPoolFactory.BuildConfigurableLinkedListPool(
							delegateContext.InitialValueAllocationCommand,
							delegateContext.AdditionalValueAllocationCommand);
				};

			return builder;
		}
	}
}