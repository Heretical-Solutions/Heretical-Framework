using HereticalSolutions.Delegates.Builders;
using HereticalSolutions.Delegates.Concurrent.Factories;

namespace HereticalSolutions.Delegates.Concurrent.Builders
{
	public static class ConcurrentBroadcasterWithRepositoryBuilder
	{
		public static ConcurrentBroadcasterWithRepository
			BuildConcurrentBroadcasterWithRepository(
				this BroadcasterWithRepositoryBuilder builder,
				ConcurrentBroadcasterFactory concurrentBroadcasterFactory)
		{
			var result = concurrentBroadcasterFactory.
				BuildConcurrentBroadcasterWithRepository(
					builder.Context.BroadcasterRepository);

			builder.Cleanup();

			return result;
		}
	}
}