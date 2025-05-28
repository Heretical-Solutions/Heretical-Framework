using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.Builders;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Concurrent.Factories
{
	public class ConcurrentMessagingFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConcurrentMessagingFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public ConcurrentMessageBus BuildConcurrentMessageBus(
			IReadOnlyRepository<Type, IPool<IMessage>> messagePoolRepository,
			BroadcasterWithRepositoryBuilder broadcasterBuilder)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentMessageBus>();

			return new ConcurrentMessageBus(
				broadcasterBuilder
					.BuildBroadcasterWithRepository(),
				messagePoolRepository,
				new Queue<IMessage>(),
				new object(),
				new object(),
				logger);
		}
	}
}