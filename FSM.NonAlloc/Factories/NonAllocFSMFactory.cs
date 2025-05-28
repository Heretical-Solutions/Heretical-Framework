using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Repositories;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.FSM.NonAlloc.Factories
{
    public class NonAllocFSMFactory
    {
        private readonly ConfigurableStackPoolFactory
            configurableStackPoolFactory;

        private readonly NonAllocBroadcasterFactory nonAllocBroadcasterFactory;

        private readonly ILoggerResolver loggerResolver;

        #region Request pool

        private const int
            DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultRequestPoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultRequestPoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        public NonAllocFSMFactory(
            ConfigurableStackPoolFactory
                configurableStackPoolFactory,
            NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
            ILoggerResolver loggerResolver)
        {
            this.configurableStackPoolFactory = configurableStackPoolFactory;

            this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

            this.loggerResolver = loggerResolver;

            defaultRequestPoolInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_REQUEST_POOL_INITIAL_ALLOCATION_AMOUNT
                };

            defaultRequestPoolAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_REQUEST_POOL_ADDITIONAL_ALLOCATION_AMOUNT
                };
        }

        public NonAllocFSMFactory(
            ConfigurableStackPoolFactory
                configurableStackPoolFactory,
            NonAllocBroadcasterFactory nonAllocBroadcasterFactory,

            AllocationCommandDescriptor
                defaultRequestPoolInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultRequestPoolAdditionalAllocationDescriptor,

            ILoggerResolver loggerResolver)
        {
            this.configurableStackPoolFactory = configurableStackPoolFactory;

            this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

            this.loggerResolver = loggerResolver;

            this.defaultRequestPoolInitialAllocationDescriptor =
                defaultRequestPoolInitialAllocationDescriptor;

            this.defaultRequestPoolAdditionalAllocationDescriptor =
                defaultRequestPoolAdditionalAllocationDescriptor;
        }

        public BaseNonAllocStateMachine<TBaseState>
            BuildBaseNonAllocStateMachine<TBaseState, TInitialState>(
                IRepository<Type, TBaseState> states,
                IRepository<Type, INonAllocTransitionEvent<TBaseState>> events,
                INonAllocTransitionController<TBaseState> transitionController)
                where TInitialState : TBaseState
                where TBaseState : INonAllocState
        {
            return BuildBaseNonAllocStateMachine<TBaseState, TInitialState>(
                states,
                events,
                transitionController,

                defaultRequestPoolInitialAllocationDescriptor,
                defaultRequestPoolAdditionalAllocationDescriptor);
        }

        public BaseNonAllocStateMachine<TBaseState>
            BuildBaseNonAllocStateMachine<TBaseState, TInitialState>(
                IRepository<Type, TBaseState> states,
                IRepository<Type, INonAllocTransitionEvent<TBaseState>> events,
                INonAllocTransitionController<TBaseState> transitionController,

                AllocationCommandDescriptor initial,
                AllocationCommandDescriptor additional)
                where TInitialState : TBaseState
                where TBaseState : INonAllocState
        {
            var logger =
                loggerResolver?.GetLogger<BaseNonAllocStateMachine<TBaseState>>();

            Func<NonAllocEventTransitionRequest>
                eventTransitionAllocationDelegate =
                    BuildNonAllocEventTransitionRequest<TBaseState>;

            Func<NonAllocImmediateTransitionRequest>
                immediateTransitionAllocationDelegate =
                    BuildNonAllocImmediateTransitionRequest<TBaseState>;

            return new BaseNonAllocStateMachine<TBaseState>(
                states,
                events,

                configurableStackPoolFactory.BuildConfigurableStackPool<
                    NonAllocEventTransitionRequest>(
                        new AllocationCommand<NonAllocEventTransitionRequest>(
                            initial,
                            eventTransitionAllocationDelegate,
                            null),
                            new AllocationCommand<NonAllocEventTransitionRequest>(
                            additional,
                            eventTransitionAllocationDelegate,
                            null)),
                configurableStackPoolFactory.BuildConfigurableStackPool<
                    NonAllocImmediateTransitionRequest>(
                        new AllocationCommand<NonAllocImmediateTransitionRequest>(
                            initial,
                            immediateTransitionAllocationDelegate,
                            null),
                            new AllocationCommand<NonAllocImmediateTransitionRequest>(
                            additional,
                            immediateTransitionAllocationDelegate,
                            null)),

                transitionController,
                new Queue<INonAllocTransitionRequest>(),

                nonAllocBroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(),
                nonAllocBroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<
                        INonAllocTransitionEvent<TBaseState>>(),

                states[typeof(TInitialState)],

                logger);
        }

        public NonAllocEventTransitionRequest 
            BuildNonAllocEventTransitionRequest<TBaseState>()
        {
            return new NonAllocEventTransitionRequest(
                nonAllocBroadcasterFactory
                    .BuildNonAllocBroadcasterGeneric<TBaseState>(),
                nonAllocBroadcasterFactory
                    .BuildNonAllocBroadcasterGeneric<TBaseState>());
        }

        public NonAllocImmediateTransitionRequest
            BuildNonAllocImmediateTransitionRequest<TBaseState>()
        {
            return new NonAllocImmediateTransitionRequest(
                nonAllocBroadcasterFactory
                    .BuildNonAllocBroadcasterGeneric<TBaseState>(),
                nonAllocBroadcasterFactory
                    .BuildNonAllocBroadcasterGeneric<TBaseState>());
        }
    }
}