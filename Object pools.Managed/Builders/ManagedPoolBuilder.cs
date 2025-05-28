using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.ObjectPools.Managed.Factories;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Builders
{
    public class ManagedPoolBuilder<T>
        : ABuilder<ManagedPoolBuilderContext<T>>
    {
        private readonly AllocationCallbackFactory allocationCallbackFactory;

        #region Default settings

        private const int
            DEFAULT_INITIAL_ALLOCATION_AMOUNT = 8;

        private const int
            DEFAULT_ADDITIONAL_ALLOCATION_AMOUNT = 8;

        protected AllocationCommandDescriptor
            defaultInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_INITIAL_ALLOCATION_AMOUNT
                };

        protected AllocationCommandDescriptor
            defaultAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_ADDITIONAL_ALLOCATION_AMOUNT
                };

        #endregion

        public ManagedPoolBuilder(
            AllocationCallbackFactory allocationCallbackFactory)
        {
            this.allocationCallbackFactory = allocationCallbackFactory;

            defaultInitialAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_INITIAL_ALLOCATION_AMOUNT
                };

            defaultAdditionalAllocationDescriptor =
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = DEFAULT_ADDITIONAL_ALLOCATION_AMOUNT
                };
        }

        public ManagedPoolBuilder(
            AllocationCallbackFactory allocationCallbackFactory,

            AllocationCommandDescriptor
                defaultInitialAllocationDescriptor,
            AllocationCommandDescriptor
                defaultAdditionalAllocationDescriptor)
        {
            this.allocationCallbackFactory = allocationCallbackFactory;


            this.defaultInitialAllocationDescriptor =
                defaultInitialAllocationDescriptor;

            this.defaultAdditionalAllocationDescriptor =
                defaultAdditionalAllocationDescriptor;
        }

        public ManagedPoolBuilder<T>
            New()
        {
            context = new ManagedPoolBuilderContext<T>
            {
                #region Allocation descriptors

                InitialAllocation = defaultInitialAllocationDescriptor,

                AdditionalAllocation = defaultAdditionalAllocationDescriptor,

                #endregion

                #region Value allocation command

                ValueAllocationDelegate =
                    AllocationFactory.ActivatorAllocationDelegate<T>,

                ValueAllocationCallbacks = new List<IAllocationCallback<T>>(),

                ResultValueAllocationCallback = null,


                InitialValueAllocationCommand = null,

                AdditionalValueAllocationCommand = null,

                #endregion

                #region Facade allocation command

                FacadeAllocationCallbacks =
                    new List<IAllocationCallback<IPoolElementFacade<T>>>(),

                ResultFacadeAllocationCallback = null,

                #endregion                

                #region Metadata allocation descriptors

                MetadataDescriptorBuilders =
                    new List<Func<MetadataAllocationDescriptor>>(),

                MetadataDescriptors = null,

                #endregion

                #region Building

                BuildDependenciesStep = true,

                InitialBuildSteps =
                    new List<Action<ManagedPoolBuilderContext<T>>>(),

                ConcretePoolBuildStep = null,

                FinalBuildSteps =
                    new List<Action<ManagedPoolBuilderContext<T>>>(),

                #endregion

                CurrentPool = null
            };

            return this;
        }

        //Ladies and gentlemen, I present to you the "BYOD" (Bring Your Own Dependency) 
        //principle. Here's what Claude 3.7 has to say about it:
        //
        //"Bring your own dependency principle" would be an approach to software design where 
        //objects or components don't create their own dependencies internally or rely on a 
        //centralized container to provide them. Instead, consumers explicitly provide all 
        //required dependencies to a component when they instantiate or use it.
        //
        //Key characteristics of this principle would include:
        //
        //Explicit Dependencies: All dependencies are visibly passed in rather than 
        //implicitly created or fetched.
        //
        //No Hidden Dependencies: Components don't reach out to some global state or service 
        //locator to obtain dependencies.
        //
        //Caller Responsibility: The caller/consumer is responsible for assembling and 
        //providing the correct dependencies.
        //
        //Composability: Since components receive their dependencies externally, they can be 
        //easily composed with different implementations.
        //
        //In practice, it's similar to manual dependency injection without a container.
        //
        // *clap* *clap* *clap*
        //
        //Thank you, Claude. Now here are some thoughts on why the hell did I create
        //a new approach to dependency injection and dared to call it a "principle":
        //
        //First, this allows to create extension methods for existing builders that may
        //require additional dependencies from sources outside of the framework (like a
        //shabadoo babadoo something factory). This way I don't have to spam builders
        //for each new dependency I want to add.
        //
        //Second, I do not have to rely on the particular IoC container of my choice
        //and force said choice on the customer. However, just having the container to
        //resolve any existing or additional dependencies would be easier
        //
        //Finally, it allows me to clearly see the dependencies required for particular
        //methods and omit introducing the ones that are not required for some simpler
        //use cases.

        #region Concrete pools

        public ManagedPoolBuilder<T>
            StackManagedPool(
                StackManagedPoolFactory stackManagedPoolFactory)
        {
            context.ConcretePoolBuildStep =
                (delegateContext) =>
                {
                    delegateContext.CurrentPool =
                        stackManagedPoolFactory.BuildStackManagedPool(
                            delegateContext.InitialValueAllocationCommand,
                            delegateContext.AdditionalValueAllocationCommand,

                            delegateContext.MetadataDescriptors,
                            delegateContext.ResultFacadeAllocationCallback);
                };

            return this;
        }

        public ManagedPoolBuilder<T>
            QueueManagedPool(
                QueueManagedPoolFactory queueManagedPoolFactory)
        {
            context.ConcretePoolBuildStep =
                (delegateContext) =>
                {
                    delegateContext.CurrentPool =
                        queueManagedPoolFactory.BuildQueueManagedPool(
                            delegateContext.InitialValueAllocationCommand,
                            delegateContext.AdditionalValueAllocationCommand,

                            delegateContext.MetadataDescriptors,
                            delegateContext.ResultFacadeAllocationCallback);
                };

            return this;
        }

        public ManagedPoolBuilder<T>
            PackedArrayManagedPool(
                PackedArrayManagedPoolFactory packedArrayManagedPoolFactory)
        {
            context.ConcretePoolBuildStep =
                (delegateContext) =>
                {
                    delegateContext.CurrentPool =
                        packedArrayManagedPoolFactory.BuildPackedArrayManagedPool(
                            delegateContext.InitialValueAllocationCommand,
                            delegateContext.AdditionalValueAllocationCommand,

                            delegateContext.MetadataDescriptors,
                            delegateContext.ResultFacadeAllocationCallback,

                            true);
                };

            return this;
        }

        public ManagedPoolBuilder<T>
            LinkedListManagedPool(
                LinkedListManagedPoolFactory linkedListManagedPoolFactory)
        {
            context.ConcretePoolBuildStep =
                (delegateContext) =>
                {
                    delegateContext.CurrentPool =
                        linkedListManagedPoolFactory.BuildLinkedListManagedPool(
                            delegateContext.InitialValueAllocationCommand,
                            delegateContext.AdditionalValueAllocationCommand,

                            delegateContext.MetadataDescriptors,
                            delegateContext.ResultFacadeAllocationCallback);
                };

            return this;
        }

        #endregion

        #region Allocation command

        public ManagedPoolBuilder<T> WithInitial(
            AllocationCommandDescriptor allocationCommand)
        {
            context.InitialAllocation = allocationCommand;

            return this;
        }

        public ManagedPoolBuilder<T> WithAdditional(
            AllocationCommandDescriptor allocationCommand)
        {
            context.AdditionalAllocation = allocationCommand;

            return this;
        }

        public ManagedPoolBuilder<T> WithActivatorAllocation()
        {
            context.ValueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<T>;

            return this;
        }

        public ManagedPoolBuilder<T> WithActivatorAllocation<TValue>()
        {
            context.ValueAllocationDelegate =
                AllocationFactory.ActivatorAllocationDelegate<T, TValue>;

            return this;
        }

        public ManagedPoolBuilder<T> WithAllocationDelegate(
            Func<T> valueAllocationDelegate)
        {
            context.ValueAllocationDelegate = valueAllocationDelegate;

            return this;
        }

        #endregion

        #region Callbacks

        public ManagedPoolBuilder<T>
            PushNewElementsToPool(
                ManagedObjectPoolAllocationCallbackFactory
                    allocationCallbackFactory)
        {
            PushToManagedPoolCallback<T> pushCallback =
                allocationCallbackFactory.
                    BuildPushToManagedPoolCallback<T>();

            context.FacadeAllocationCallbacks.Add(
                pushCallback);

            context.FinalBuildSteps.Add(
                (context) =>
                {
                    pushCallback.TargetPool =
                        context.CurrentPool;
                });

            return this;
        }

        public ManagedPoolBuilder<T>
            PushNewElementsToPoolWhenAvailable(
                ManagedObjectPoolAllocationCallbackFactory
                    allocationCallbackFactory)
        {
            PushToManagedPoolWhenAvailableCallback<T> pushCallback =
                allocationCallbackFactory.
                    BuildPushToManagedPoolWhenAvailableCallback<T>();

            context.FacadeAllocationCallbacks.Add(pushCallback);

            context.FinalBuildSteps.Add(
                (context) =>
                {
                    pushCallback.TargetPool =
                        context.CurrentPool;
                });

            return this;
        }

        #endregion

        public IManagedPool<T> Build()
        {
            if (context.BuildDependenciesStep)
                BuildDependencies();

            foreach (var buildStep in context.InitialBuildSteps)
            {
                buildStep?.Invoke(
                    context);
            }

            context.ConcretePoolBuildStep?.Invoke(
                context);

            foreach (var buildStep in context.FinalBuildSteps)
            {
                buildStep?.Invoke(
                    context);
            }

            var result = context.CurrentPool;

            Cleanup();

            return result;
        }

        public void BuildDependencies()
        {
            context.MetadataDescriptors = BuildMetadataDescriptors();

            context.ResultFacadeAllocationCallback =
                BuildCompositeFacadeAllocationCallback();

            context.ResultValueAllocationCallback =
                BuildCompositeValueAllocationCallback();

            context.InitialValueAllocationCommand =
                new AllocationCommand<T>(
                    context.InitialAllocation,
                    context.ValueAllocationDelegate,
                    context.ResultValueAllocationCallback);

            context.AdditionalValueAllocationCommand =
                new AllocationCommand<T>(
                    context.AdditionalAllocation,
                    context.ValueAllocationDelegate,
                    context.ResultValueAllocationCallback);
        }

        private MetadataAllocationDescriptor[] BuildMetadataDescriptors()
        {
            List<MetadataAllocationDescriptor> metadataDescriptorsList =
                new List<MetadataAllocationDescriptor>();

            foreach (var descriptorBuilder in context.MetadataDescriptorBuilders)
            {
                if (descriptorBuilder != null)
                    metadataDescriptorsList.Add(
                        descriptorBuilder?.Invoke());
            }

            return metadataDescriptorsList.ToArray();
        }

        private IAllocationCallback<IPoolElementFacade<T>>
            BuildCompositeFacadeAllocationCallback()
        {
            IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null;

            if (context.FacadeAllocationCallbacks != null
                && context.FacadeAllocationCallbacks.Count > 0)
            {
                //The list is reversed for the purpose to call decorator's callbacks
                //first. Edge case: I need to add address metadata to decorator
                //before pushing it, otherwise the address decorator has no idea which
                //of the internal pools it should push to.
                context.FacadeAllocationCallbacks.Reverse();

                facadeAllocationCallback =
                    allocationCallbackFactory.
                        BuildCompositeCallback<IPoolElementFacade<T>>(
                            context.FacadeAllocationCallbacks);
            }

            return facadeAllocationCallback;
        }

        private IAllocationCallback<T> BuildCompositeValueAllocationCallback()
        {
            IAllocationCallback<T> valueAllocationCallback = null;

            if (context.ValueAllocationCallbacks != null
                && context.ValueAllocationCallbacks.Count > 0)
            {
                //The list is reversed for the purpose to call decorator's callbacks
                //first
                context.ValueAllocationCallbacks.Reverse();

                valueAllocationCallback =
                    allocationCallbackFactory.BuildCompositeCallback<T>(
                        context.ValueAllocationCallbacks);
            }

            return valueAllocationCallback;
        }
    }
}