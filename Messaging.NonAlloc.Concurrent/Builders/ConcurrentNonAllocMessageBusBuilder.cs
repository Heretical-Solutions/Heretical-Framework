using HereticalSolutions.Messaging.NonAlloc.Builders;

using HereticalSolutions.Messaging.NonAlloc.Concurrent.Factories;

namespace HereticalSolutions.Messaging.NonAlloc.Concurrent.Builders
{
	public static class ConcurrentNonAllocMessageBusBuilder
	{
		public static ConcurrentNonAllocMessageBus
			BuildConcurrentNonAllocMessageBus(
				this NonAllocMessageBusBuilder builder,
				ConcurrentNonAllocMessagingFactory
					concurrentNonAllocMessagingFactory)
		{
			var context = builder.Context;

			var result = concurrentNonAllocMessagingFactory.
				BuildConcurrentNonAllocMessageBus(
					context.MessagePoolRepository,
					context.BroadcasterBuilder);

			builder.Cleanup();

			return result;
		}
	}
}