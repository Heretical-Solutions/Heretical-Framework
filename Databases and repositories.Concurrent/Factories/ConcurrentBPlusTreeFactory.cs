using System.Threading;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Repositories.Concurrent.Factories
{
	public class ConcurrentBPlusTreeFactory
	{
		private readonly BPlusTreeFactory bPlusTreeFactory;

		public ConcurrentBPlusTreeFactory(
			BPlusTreeFactory bPlusTreeFactory)
		{
			this.bPlusTreeFactory = bPlusTreeFactory;
		}

		#region Concurrent B+ tree repository

		public ConcurrentBPlusTreeRepository<TKey, TValue>
			BuildConcurrentBPlusTreeRepository<TKey, TValue>()
		{
			return new ConcurrentBPlusTreeRepository<TKey, TValue>(
				bPlusTreeFactory.BuildBPlusTreeRepository<TKey, TValue>(),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentBPlusTreeRepository<TKey, TValue>
			BuildConcurrentBPlusTreeRepository<TKey, TValue>(
				BPlusTreeRepository<TKey, TValue> database)
		{
			return new ConcurrentBPlusTreeRepository<TKey, TValue>(
				database,
				new SemaphoreSlim(1, 1));
		}

		#endregion
	}
}