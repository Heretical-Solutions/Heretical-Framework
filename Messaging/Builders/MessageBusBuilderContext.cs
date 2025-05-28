using System;

using HereticalSolutions.Delegates.Builders;

using HereticalSolutions.Repositories;

using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.Messaging.Builders
{
	public class MessageBusBuilderContext
	{
		public IRepository<Type, IPool<IMessage>> MessagePoolRepository;

		public BroadcasterWithRepositoryBuilder BroadcasterWithRepositoryBuilder;
	}
}