using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories.Concurrent.Factories
{
	public class ConcurrentOneToOneMapFactory
	{
		private readonly BPlusTreeFactory bPlusTreeFactory;

		private readonly ILoggerResolver loggerResolver;

		public ConcurrentOneToOneMapFactory(
			BPlusTreeFactory bPlusTreeFactory,
			ILoggerResolver loggerResolver)
		{
			this.bPlusTreeFactory = bPlusTreeFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Concurrent dictionary one to one map

		public ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
			BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>()
		{
			ILogger logger = loggerResolver
				?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

			return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(),
				new Dictionary<TValue2, TValue1>(),
				this,
				new object(),
				new object(),
				logger);
		}

		public ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
			BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				Dictionary<TValue1, TValue2> leftDatabase,
				Dictionary<TValue2, TValue1> rightDatabase)
		{
			ILogger logger = loggerResolver
				?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

			return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				leftDatabase,
				rightDatabase,
				this,
				new object(),
				new object(),
				logger);
		}

		public ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
			BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				IEqualityComparer<TValue1> leftComparer,
				IEqualityComparer<TValue2> rightComparer)
		{
			ILogger logger = loggerResolver
				?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

			return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(leftComparer),
				new Dictionary<TValue2, TValue1>(rightComparer),
				this,
				new object(),
				new object(),
				logger);
		}

		public ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
			CloneConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				Dictionary<TValue1, TValue2> leftToRightDatabase,
				Dictionary<TValue2, TValue1> rightToLeftDatabase,
				ILogger logger)
		{
			return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
				new Dictionary<TValue1, TValue2>(
					leftToRightDatabase,
					leftToRightDatabase.Comparer),
				new Dictionary<TValue2, TValue1>(
					rightToLeftDatabase,
					rightToLeftDatabase.Comparer),
				this,
				new object(),
				new object(),
				logger);
		}

		#endregion
	}
}