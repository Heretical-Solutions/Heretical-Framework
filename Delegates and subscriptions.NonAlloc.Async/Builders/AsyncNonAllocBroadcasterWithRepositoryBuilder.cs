using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.Delegates.NonAlloc.Async.Factories;

namespace HereticalSolutions.Delegates.NonAlloc.Async.Builders
{
	public static class AsyncBroadcasterWithRepositoryBuilder
	{
		public static AsyncNonAllocBroadcasterWithRepository
			BuildConcurrentNonAllocBroadcasterWithRepository(
				this NonAllocBroadcasterWithRepositoryBuilder builder,
				AsyncNonAllocBroadcasterFactory
					asyncNonAllocBroadcasterFactory)
		{
			var result = asyncNonAllocBroadcasterFactory.
				BuildAsyncNonAllocBroadcasterWithRepository(
					builder.Context.BroadcasterRepository);

			builder.Cleanup();

			return result;
		}
	}
}