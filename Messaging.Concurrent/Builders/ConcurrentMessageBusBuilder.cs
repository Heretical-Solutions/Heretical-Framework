using HereticalSolutions.Messaging.Builders;

using HereticalSolutions.Messaging.Concurrent.Factories;

namespace HereticalSolutions.Messaging.Concurrent.Builders
{
	public static class ConcurrentMessageBusBuilder
	{
		public static ConcurrentMessageBus
			BuildConcurrentMessageBus(
				this MessageBusBuilder builder,
				ConcurrentMessagingFactory
					concurrentMessagingFactory)
		{
			var context = builder.Context;

			var result = concurrentMessagingFactory.
				BuildConcurrentMessageBus(
					context.MessagePoolRepository,
					context.BroadcasterWithRepositoryBuilder);

			builder.Cleanup();

			return result;
		}
	}
}