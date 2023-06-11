using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Messaging.Factories
{
    public class NonAllocMessageBusBuilder
    {
        private readonly IObjectRepository messagePoolRepository;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;
        
        public NonAllocMessageBusBuilder()
        {
            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new NonAllocBroadcasterWithRepositoryBuilder();
        }

        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;
            
            INonAllocDecoratedPool<IMessage> messagePool = PoolsFactory.BuildResizableNonAllocPool<IMessage>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public NonAllocMessageBus Build()
        {
            Func<IPoolElement<IMessage>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IPoolElement<IMessage>>;
            
            INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox = PoolsFactory.BuildResizableNonAllocPool<IPoolElement<IMessage>>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });
            
            var mailboxContents = ((IModifiable<INonAllocPool<IPoolElement<IMessage>>>)mailbox).Contents;
            
            var mailboxContentAsIndexable = (IIndexable<IPoolElement<IPoolElement<IMessage>>>)mailboxContents;
            
            return new NonAllocMessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                mailbox,
                mailboxContentAsIndexable);
        }
    }
}