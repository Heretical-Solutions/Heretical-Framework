using HereticalSolutions.ObjectPools.Async.Builders;
using HereticalSolutions.ObjectPools.Configurable.Async.Factories;

namespace HereticalSolutions.ObjectPools.Configurable.Async.Builders
{
	public static class AsyncConfigurablePoolBuilder
	{
		public static AsyncPoolBuilder<T> AsyncConfigurableStackPool<T>(
			this AsyncPoolBuilder<T> builder,
			AsyncConfigurableStackPoolFactory asyncConfigurableStackPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncConfigurableStackPoolFactory.
							BuildAsyncConfigurableStackPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext);
				};

			return builder;
		}

		public static AsyncPoolBuilder<T> AsyncConfigurableQueuePool<T>(
			this AsyncPoolBuilder<T> builder,
			AsyncConfigurableQueuePoolFactory asyncConfigurableQueuePoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncConfigurableQueuePoolFactory.
							BuildAsyncConfigurableQueuePool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext);
				};

			return builder;
		}

		public static AsyncPoolBuilder<T> AsyncConfigurablePackedArrayPool<T>(
			this AsyncPoolBuilder<T> builder,
			AsyncConfigurablePackedArrayPoolFactory asyncConfigurablePackedArrayPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncConfigurablePackedArrayPoolFactory.
							BuildAsyncConfigurablePackedArrayPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext);
				};

			return builder;
		}

		public static AsyncPoolBuilder<T> AsyncConfigurableLinkedListPool<T>(
			this AsyncPoolBuilder<T> builder,
			AsyncConfigurableLinkedListPoolFactory asyncConfigurableLinkedListPoolFactory)
		{
			var context = builder.Context;

			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncConfigurableLinkedListPoolFactory.
							BuildAsyncConfigurableLinkedListPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext);
				};

			return builder;
		}
	}
}