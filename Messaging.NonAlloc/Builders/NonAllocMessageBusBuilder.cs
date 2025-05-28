using System;

using HereticalSolutions.Allocations;
//using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.NonAlloc.Builders;

using HereticalSolutions.ObjectPools.Managed;
using HereticalSolutions.ObjectPools.Managed.Factories;
using HereticalSolutions.ObjectPools.Managed.Builders;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.Messaging.NonAlloc.Factories;

namespace HereticalSolutions.Messaging.NonAlloc.Builders
{
    public class NonAllocMessageBusBuilder
        : ABuilder<NonAllocMessageBusBuilderContext>
    {
        private readonly RepositoryFactory repositoryFactory;
        
        private readonly ManagedPoolBuilder<IMessage> messagePoolBuilder;
        
        private readonly PackedArrayManagedPoolFactory packedArrayManagedPoolFactory;

        private readonly NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder;

        private readonly NonAllocMessagingFactory nonAllocMessagingFactory;

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

        public NonAllocMessageBusBuilder(
            RepositoryFactory repositoryFactory,
            ManagedPoolBuilder<IMessage> messagePoolBuilder,
            PackedArrayManagedPoolFactory packedArrayManagedPoolFactory,
            NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder,
            NonAllocMessagingFactory nonAllocMessagingFactory)
        {
            this.repositoryFactory = repositoryFactory;

            this.messagePoolBuilder = messagePoolBuilder;

            this.packedArrayManagedPoolFactory = packedArrayManagedPoolFactory;

            this.broadcasterBuilder = broadcasterBuilder;

            this.nonAllocMessagingFactory = nonAllocMessagingFactory;

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

        public NonAllocMessageBusBuilder(
            RepositoryFactory repositoryFactory,
            ManagedPoolBuilder<IMessage> messagePoolBuilder,
            PackedArrayManagedPoolFactory packedArrayManagedPoolFactory,
            NonAllocBroadcasterWithRepositoryBuilder broadcasterBuilder,
            NonAllocMessagingFactory nonAllocMessagingFactory,

            AllocationCommandDescriptor
                defaultMessagePoolInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultMessagePoolAdditionalAllocationDescriptor)
        {
            this.repositoryFactory = repositoryFactory;

            this.messagePoolBuilder = messagePoolBuilder;

            this.packedArrayManagedPoolFactory = packedArrayManagedPoolFactory;

            this.broadcasterBuilder = broadcasterBuilder;

            this.nonAllocMessagingFactory = nonAllocMessagingFactory;


            this.defaultMessagePoolInitialAllocationDescriptor =
                defaultMessagePoolInitialAllocationDescriptor;

            this.defaultMessagePoolAdditionalAllocationDescriptor =
                defaultMessagePoolAdditionalAllocationDescriptor;
        }

        public NonAllocMessageBusBuilder New()
        {
            context = new NonAllocMessageBusBuilderContext
            {
                MessagePoolRepository =
                    repositoryFactory
                        .BuildDictionaryRepository<Type, IManagedPool<IMessage>>(),

                BroadcasterBuilder = broadcasterBuilder
            };

            context.BroadcasterBuilder.New();

            return this;
        }

        public NonAllocMessageBusBuilder AddMessageType<TMessage>()
        {
            //Func<IMessage> valueAllocationDelegate =
            //    AllocationFactory.ActivatorAllocationDelegate<IMessage, TMessage>;

            IManagedPool<IMessage> messagePool = messagePoolBuilder
                .New()
                .PackedArrayManagedPool(
                    packedArrayManagedPoolFactory)
                .WithInitial(
                    defaultMessagePoolInitialAllocationDescriptor)
                .WithAdditional(
                    defaultMessagePoolAdditionalAllocationDescriptor)
                //.WithAllocationDelegate(
                //    valueAllocationDelegate)
                .WithActivatorAllocation<TMessage>()
                .Build();
            
            context.MessagePoolRepository.Add(
                typeof(TMessage),
                messagePool);

            context.BroadcasterBuilder.Add<TMessage>();

            return this;
        }

        public NonAllocMessageBus BuildNonAllocMessageBus()
        {
            var result = nonAllocMessagingFactory.BuildNonAllocMessageBus(
                context.MessagePoolRepository,
                context.BroadcasterBuilder);

            Cleanup();

            return result;
        }
    }
}