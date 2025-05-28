using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocMessagingFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConcurrentNonAllocMessagingFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public ConcurrentNonAllocMessageBus BuildConcurrentNonAllocMessageBus(
			IReadOnlyRepository<Type, IManagedPool<IMessage>> messagePoolRepository,
			NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentNonAllocMessageBus>();

			return new ConcurrentNonAllocMessageBus(
				broadcasterBuilder
					.BuildNonAllocBroadcasterWithRepository(),
				messagePoolRepository,
				new Queue<IPoolElementFacade<IMessage>>(),
				new object(),
				new object(),
				logger);
		}
	}
}