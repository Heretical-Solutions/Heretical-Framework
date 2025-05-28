using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories.Factories
{
	public class BPlusTreeFactory
	{
		private readonly CollectionFactory collectionFactory;

		private readonly ILoggerResolver loggerResolver;

		public BPlusTreeFactory(
			CollectionFactory collectionFactory,
			ILoggerResolver loggerResolver)
		{
			this.collectionFactory = collectionFactory;

			this.loggerResolver = loggerResolver;
		}
		
		#region B+ tree repository

		public BPlusTreeRepository<TKey, TValue>
			BuildBPlusTreeRepository<TKey, TValue>()
		{
			ILogger logger = loggerResolver
				?.GetLogger<BPlusTreeRepository<TKey, TValue>>();

			return new BPlusTreeRepository<TKey, TValue>(
				collectionFactory.BuildBPlusTreeMap<TKey, TValue>(),
				logger);
		}

		public BPlusTreeRepository<TKey, TValue>
			BuildBPlusTreeRepository<TKey, TValue>(
				IBPlusTreeMap<TKey, TValue> database)
		{
			ILogger logger = loggerResolver
				?.GetLogger<BPlusTreeRepository<TKey, TValue>>();

			return new BPlusTreeRepository<TKey, TValue>(
				database,
				logger);
		}

		#endregion
	}
}