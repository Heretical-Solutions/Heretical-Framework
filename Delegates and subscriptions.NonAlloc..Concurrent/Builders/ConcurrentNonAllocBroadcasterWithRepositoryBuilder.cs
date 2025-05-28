using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.Delegates.NonAlloc.Concurrent.Factories;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent.Builders
{
	public static class ConcurrentNonAllocBroadcasterWithRepositoryBuilder
	{
		public static ConcurrentNonAllocBroadcasterWithRepository
			BuildConcurrentNonAllocBroadcasterWithRepository(
				this NonAllocBroadcasterWithRepositoryBuilder builder,
				ConcurrentNonAllocBroadcasterFactory 
					concurrentNonAllocBroadcasterFactory)
		{
			var result = concurrentNonAllocBroadcasterFactory.
				BuildConcurrentNonAllocBroadcasterWithRepository(
					builder.Context.BroadcasterRepository);

			builder.Cleanup();

			return result;
		}
	}
}