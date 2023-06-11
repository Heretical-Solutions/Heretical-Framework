using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Messaging.Factories
{
    public class MessageBusBuilder
    {
        private readonly IObjectRepository messagePoolRepository;

        private readonly BroadcasterWithRepositoryBuilder broadcasterBuilder;
        
        public MessageBusBuilder()
        {
            messagePoolRepository = RepositoriesFactory.BuildDictionaryObjectRepository();

            broadcasterBuilder = new BroadcasterWithRepositoryBuilder();
        }

        public MessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate = AllocationsFactory.ActivatorAllocationDelegate<IMessage, TMessage>;

            var initialAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },

                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IMessage>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                },

                AllocationDelegate = valueAllocationDelegate
            };
            
            IPool<IMessage> messagePool = PoolsFactory.BuildStackPool<IMessage>(
                initialAllocationCommand,
                additionalAllocationCommand);
            
            messagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            broadcasterBuilder.Add<TMessage>();

            return this;
        }

        public MessageBus Build()
        {
            return new MessageBus(
                broadcasterBuilder.Build(),
                (IReadOnlyObjectRepository)messagePoolRepository,
                new Queue<IMessage>());
        }
    }
}