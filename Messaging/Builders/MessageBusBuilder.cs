using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Builders;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.Messaging.Factories;

namespace HereticalSolutions.Messaging.Builders
{
    public class MessageBusBuilder
        : ABuilder<MessageBusBuilderContext>
    {
        private readonly RepositoryFactory repositoryFactory;

        private readonly ConfigurableStackPoolFactory stackPoolFactory;

        private readonly BroadcasterWithRepositoryBuilder 
            broadcasterWithRepositoryBuilder;

        private readonly MessagingFactory messagingFactory;

        #region Message pool

        private const int
            DEFAULT_MESSAGE_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_MESSAGE_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultMessagePoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultMessagePoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        public MessageBusBuilder(
            RepositoryFactory repositoryFactory,
            ConfigurableStackPoolFactory stackPoolFactory,
            BroadcasterWithRepositoryBuilder broadcasterWithRepositoryBuilder,
            MessagingFactory messagingFactory)
        {
            this.repositoryFactory = repositoryFactory;

            this.stackPoolFactory = stackPoolFactory;

            this.broadcasterWithRepositoryBuilder = broadcasterWithRepositoryBuilder;

            this.messagingFactory = messagingFactory;

            defaultMessagePoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_INITIAL_ALLOCATION_AMOUNT
                };

            defaultMessagePoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_MESSAGE_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };
        }

        public MessageBusBuilder(
            RepositoryFactory repositoryFactory,
            ConfigurableStackPoolFactory stackPoolFactory,
            BroadcasterWithRepositoryBuilder broadcasterWithRepositoryBuilder,
            MessagingFactory messagingFactory,

            AllocationCommandDescriptor
                defaultMessagePoolInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultMessagePoolAdditionalAllocationDescriptor)
        {
            this.repositoryFactory = repositoryFactory;

            this.stackPoolFactory = stackPoolFactory;

            this.broadcasterWithRepositoryBuilder = broadcasterWithRepositoryBuilder;

            this.messagingFactory = messagingFactory;


            this.defaultMessagePoolInitialAllocationDescriptor =
                defaultMessagePoolInitialAllocationDescriptor;

            this.defaultMessagePoolAdditionalAllocationDescriptor =
                defaultMessagePoolAdditionalAllocationDescriptor;
        }

        public MessageBusBuilder New()
        {
            context = new MessageBusBuilderContext
            {
                MessagePoolRepository =
                    repositoryFactory
                        .BuildDictionaryRepository<Type, IPool<IMessage>>(),

                BroadcasterWithRepositoryBuilder = broadcasterWithRepositoryBuilder
            };

            context.BroadcasterWithRepositoryBuilder.New();

            return this;
        }

        public MessageBusBuilder AddMessageType<TMessage>()
        {
            Func<IMessage> valueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<IMessage, TMessage>;

            var initialAllocationCommand = new AllocationCommand<IMessage>(
                defaultMessagePoolInitialAllocationDescriptor,
                valueAllocationDelegate,
                null);
            
            var additionalAllocationCommand = new AllocationCommand<IMessage>(
                defaultMessagePoolAdditionalAllocationDescriptor,
                valueAllocationDelegate,
                null);
            
            IPool<IMessage> messagePool = stackPoolFactory.
                BuildConfigurableStackPool<IMessage>(
                    initialAllocationCommand,
                    additionalAllocationCommand);
            
            context.MessagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            context.BroadcasterWithRepositoryBuilder.Add<TMessage>();

            return this;
        }

        public MessageBus BuildMessageBus()
        {
            var result = messagingFactory.BuildMessageBus(
                context.MessagePoolRepository,
                context.BroadcasterWithRepositoryBuilder);

            Cleanup();

            return result;
        }
    }
}