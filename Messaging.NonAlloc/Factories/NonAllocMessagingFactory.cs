using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.NonAlloc.Factories
{
	public class NonAllocMessagingFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public NonAllocMessagingFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public NonAllocMessageBus BuildNonAllocMessageBus(
			IReadOnlyRepository<Type, IManagedPool<IMessage>> messagePoolRepository,
			NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder)
		{
			ILogger logger =
				loggerResolver?.GetLogger<NonAllocMessageBus>();

			return new NonAllocMessageBus(
				broadcasterBuilder
					.BuildNonAllocBroadcasterWithRepository(),
				messagePoolRepository,
				new Queue<IPoolElementFacade<IMessage>>(),
				logger);
		}
	}
}