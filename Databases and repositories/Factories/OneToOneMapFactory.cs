using System.Collections.Generic;

using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories.Factories
{
	public class OneToOneMapFactory
	{
		private readonly CollectionFactory collectionFactory;

		private readonly ILoggerResolver loggerResolver;

		public OneToOneMapFactory(
			CollectionFactory collectionFactory,
			ILoggerResolver loggerResolver)
		{
			this.collectionFactory = collectionFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Dictionary one to one map

		public DictionaryOneToOneMap<TValue1, TValue2>
			BuildDictionaryOneToOneMap<TValue1, TValue2>()
		{
			ILogger logger = loggerResolver
				?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

			return new DictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(),
				new Dictionary<TValue2, TValue1>(),
				this,
				logger);
		}

		public DictionaryOneToOneMap<TValue1, TValue2>
			BuildDictionaryOneToOneMap<TValue1, TValue2>(
				Dictionary<TValue1, TValue2> leftDatabase,
				Dictionary<TValue2, TValue1> rightDatabase)
		{
			ILogger logger = loggerResolver
				?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

			return new DictionaryOneToOneMap<TValue1, TValue2>(
				leftDatabase,
				rightDatabase,
				this,
				logger);
		}

		public DictionaryOneToOneMap<TValue1, TValue2>
			BuildDictionaryOneToOneMap<TValue1, TValue2>(
				IEqualityComparer<TValue1> leftComparer,
				IEqualityComparer<TValue2> rightComparer)
		{
			ILogger logger = loggerResolver
				?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

			return new DictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(leftComparer),
				new Dictionary<TValue2, TValue1>(rightComparer),
				this,
				logger);
		}

		public DictionaryOneToOneMap<TValue1, TValue2>
			CloneDictionaryOneToOneMap<TValue1, TValue2>(
				Dictionary<TValue1, TValue2> leftToRightDatabase,
				Dictionary<TValue2, TValue1> rightToLeftDatabase,
				ILogger logger)
		{
			return new DictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(
					leftToRightDatabase,
					leftToRightDatabase.Comparer),
				new Dictionary<TValue2, TValue1>(
					rightToLeftDatabase,
					rightToLeftDatabase.Comparer),
				this,
				logger);
		}

		#endregion
	}
}