using System;

using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.Repositories;

using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.Messaging.NonAlloc.Builders
{
	public class NonAllocMessageBusBuilderContext
	{
		public IRepository<Type, IManagedPool<IMessage>> MessagePoolRepository;

		public NonAllocBroadcasterWithRepositoryBuilder BroadcasterBuilder;
		
	}
}