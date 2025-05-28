using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.Builders;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Factories
{
	public class MessagingFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public MessagingFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public MessageBus BuildMessageBus(
			IReadOnlyRepository<Type, IPool<IMessage>> messagePoolRepository,
			BroadcasterWithRepositoryBuilder broadcasterBuilder)
		{
			ILogger logger =
				loggerResolver?.GetLogger<MessageBus>();

			return new MessageBus(
				broadcasterBuilder
					.BuildBroadcasterWithRepository(),
				messagePoolRepository,
				new Queue<IMessage>(),
				logger);
		}
	}
}