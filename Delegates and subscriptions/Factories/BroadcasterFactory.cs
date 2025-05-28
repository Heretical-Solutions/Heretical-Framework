using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class BroadcasterFactory
    {
        private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

        private readonly ILoggerResolver loggerResolver;

        #region Broadcaster context pool

        private const int
            DEFAULT_BROADCASTER_CONTEXT_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_BROADCASTER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultBroadcasterContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
    
                    Amount = DEFAULT_BROADCASTER_CONTEXT_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultBroadcasterContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
    
                    Amount = DEFAULT_BROADCASTER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        #region Broadcaster subscriptions pool

        private const int
            DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultSubscriptionPoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultSubscriptionPoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        #region Invocation context

        private const int DEFAULT_INVOKATION_CONTEXT_SIZE = 32;

        protected int invokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;

        #endregion

        public BroadcasterFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory,
            ILoggerResolver loggerResolver)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            defaultBroadcasterContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
    
                    Amount = DEFAULT_BROADCASTER_CONTEXT_INITIAL_ALLOCATION_AMOUNT
                };

            defaultBroadcasterContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
    
                    Amount = DEFAULT_BROADCASTER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT
                };


            defaultSubscriptionPoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_SUBSCRIPTION_POOL_INITIAL_ALLOCATION_AMOUNT
                };

            defaultSubscriptionPoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_SUBSCRIPTION_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };


            invokationContextSize = DEFAULT_INVOKATION_CONTEXT_SIZE;

            this.loggerResolver = loggerResolver;
        }

        public BroadcasterFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory,

            AllocationCommandDescriptor
                defaultBroadcasterContextInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultBroadcasterContextAdditionalAllocationDescriptor,

            AllocationCommandDescriptor
                defaultSubscriptionPoolInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultSubscriptionPoolAdditionalAllocationDescriptor,

            int invokationContextSize,

            ILoggerResolver loggerResolver)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            this.defaultBroadcasterContextInitialAllocationDescriptor =
                defaultBroadcasterContextInitialAllocationDescriptor;
            
            this.defaultBroadcasterContextAdditionalAllocationDescriptor =
                defaultBroadcasterContextAdditionalAllocationDescriptor;


            this.defaultSubscriptionPoolInitialAllocationDescriptor =
                defaultSubscriptionPoolInitialAllocationDescriptor;

            this.defaultSubscriptionPoolAdditionalAllocationDescriptor =
                defaultSubscriptionPoolAdditionalAllocationDescriptor;


            this.invokationContextSize = invokationContextSize;

            this.loggerResolver = loggerResolver;
        }

        public BroadcasterFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory,

            int defaultBroadcasterContextInitialAllocationAmount,
            int defaultBroadcasterContextAdditionalAllocationAmount,

            int defaultSubscriptionPoolInitialAllocationAmount,
            int defaultSubscriptionPoolAdditionalAllocationAmount,

            int invokationContextSize,

            ILoggerResolver loggerResolver)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            defaultBroadcasterContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultBroadcasterContextInitialAllocationAmount
                };

            defaultBroadcasterContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultBroadcasterContextAdditionalAllocationAmount
                };


            defaultSubscriptionPoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultSubscriptionPoolInitialAllocationAmount
                };

            defaultSubscriptionPoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultSubscriptionPoolAdditionalAllocationAmount
                };


            this.invokationContextSize = invokationContextSize;

            this.loggerResolver = loggerResolver;
        }

        public IPool<T> BuildContextPool<T>()
        {
            Func<T> valueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<T>;

            return configurableStackPoolFactory.BuildConfigurableStackPool<T>(
                new AllocationCommand<T>(
                    defaultBroadcasterContextInitialAllocationDescriptor,
                    valueAllocationDelegate,
                    null),
                new AllocationCommand<T>(
                    defaultBroadcasterContextAdditionalAllocationDescriptor,
                    valueAllocationDelegate,
                    null));
        }

        #region Broadcaster generic

        public BroadcasterGeneric<T> BuildBroadcasterGeneric<T>()
        {
            var contextPool = BuildContextPool<BroadcasterInvocationContext<T>>();

            return BuildBroadcasterGeneric<T>(
                contextPool);
        }

        public BroadcasterGeneric<T> BuildBroadcasterGeneric<T>(
            IPool<BroadcasterInvocationContext<T>> contextPool)
        {
            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterGeneric<T>>();

            return new BroadcasterGeneric<T>(
                contextPool,
                logger);
        }

        #endregion

        #region Broadcaster multiple args

        public BroadcasterMultipleArgs BuildBroadcasterMultipleArgs()
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>());
        }

        public BroadcasterMultipleArgs BuildBroadcasterMultipleArgs(
            IPool<BroadcasterInvocationContext<object[]>> contextPool)
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>(
                    contextPool));
        }

        #endregion

        #region Broadcaster with repository

        public BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcasterRepository)
        {
            ILogger logger =
                loggerResolver?.GetLogger<BroadcasterWithRepository>();

            return new BroadcasterWithRepository(
                broadcasterRepository,
                logger);
        }

        #endregion
    }
}