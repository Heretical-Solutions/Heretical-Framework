using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public class PingerFactory
    {
        private readonly ConfigurableStackPoolFactory configurableStackPoolFactory;

        #region Pinger context pool

        private const int
            DEFAULT_PINGER_CONTEXT_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_PINGER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultPingerContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_PINGER_CONTEXT_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultPingerContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_PINGER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        public PingerFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            defaultPingerContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_PINGER_CONTEXT_INITIAL_ALLOCATION_AMOUNT
                };

            defaultPingerContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_PINGER_CONTEXT_ADDITIONAL_ALLOCATION_AMOUNT
                };
        }

        public PingerFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory,

            AllocationCommandDescriptor
                defaultPingerContextInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultPingerContextAdditionalAllocationDescriptor)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            this.defaultPingerContextInitialAllocationDescriptor =
                defaultPingerContextInitialAllocationDescriptor;

            this.defaultPingerContextAdditionalAllocationDescriptor =
                defaultPingerContextAdditionalAllocationDescriptor;
        }

        public PingerFactory(
            ConfigurableStackPoolFactory configurableStackPoolFactory,

            int defaultPingerContextInitialAllocationAmount,
            int defaultPingerContextAdditionalAllocationAmount)
        {
            this.configurableStackPoolFactory =
                configurableStackPoolFactory;

            defaultPingerContextInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultPingerContextInitialAllocationAmount
                };

            defaultPingerContextAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = defaultPingerContextAdditionalAllocationAmount
                };
        }

        public IPool<T> BuildContextPool<T>()
        {
            Func<T> valueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<T>;

            return configurableStackPoolFactory.BuildConfigurableStackPool<T>(
                new AllocationCommand<T>(
                    defaultPingerContextInitialAllocationDescriptor,
                    valueAllocationDelegate,
                    null),
                new AllocationCommand<T>(
                    defaultPingerContextAdditionalAllocationDescriptor,
                    valueAllocationDelegate,
                    null));
        }

        #region Pinger

        public Pinger BuildPinger()
        {
            return BuildPinger(
                BuildContextPool<PingerInvocationContext>());
        }

        public Pinger BuildPinger(
            IPool<PingerInvocationContext> contextPool)
        {
            return new Pinger(
                contextPool);
        }

        #endregion
    }
}