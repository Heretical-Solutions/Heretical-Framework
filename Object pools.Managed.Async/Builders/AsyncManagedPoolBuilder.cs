using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Allocations.Async;
using HereticalSolutions.Allocations.Async.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.ObjectPools.Managed.Async.Factories;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Async.Builders
{
	public class AsyncManagedPoolBuilder<T>
		: ABuilder<AsyncManagedPoolBuilderContext<T>>
	{
		private readonly AsyncAllocationCallbackFactory asyncAllocationCallbackFactory;

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

		public AsyncManagedPoolBuilder(
			AsyncAllocationCallbackFactory asyncAllocationCallbackFactory)
		{
			this.asyncAllocationCallbackFactory = asyncAllocationCallbackFactory;

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

		public AsyncManagedPoolBuilder(
			AsyncAllocationCallbackFactory asyncAllocationCallbackFactory,

			AllocationCommandDescriptor
				defaultInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultAdditionalAllocationDescriptor)
		{
			this.asyncAllocationCallbackFactory = asyncAllocationCallbackFactory;


			this.defaultInitialAllocationDescriptor =
				defaultInitialAllocationDescriptor;

			this.defaultAdditionalAllocationDescriptor =
				defaultAdditionalAllocationDescriptor;
		}

		public AsyncManagedPoolBuilder<T>
			New()
		{
			context = new AsyncManagedPoolBuilderContext<T>
			{
				#region Allocation descriptors

				InitialAllocation = defaultInitialAllocationDescriptor,

				AdditionalAllocation = defaultAdditionalAllocationDescriptor,

				#endregion

				#region Value allocation command

				ValueAllocationDelegate =
					async () =>
					{
						var result = AllocationFactory.ActivatorAllocationDelegate<T>();

						return result;
					},

				ValueAllocationCallbacks = new List<IAsyncAllocationCallback<T>>(),

				ResultValueAllocationCallback = null,


				InitialValueAllocationCommand = null,

				AdditionalValueAllocationCommand = null,

				#endregion

				#region Facade allocation command

				FacadeAllocationCallbacks =
					new List<IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>>(),

				ResultFacadeAllocationCallback = null,

				#endregion

				#region Metadata allocation descriptors

				MetadataDescriptorBuilders =
					new List<Func<MetadataAllocationDescriptor>>(),

				MetadataDescriptors = null,

				#endregion

				#region Building

				InitialBuildSteps =
					new List<BuildStepAsync<T>>(),

				ConcretePoolBuildStep = null,

				FinalBuildSteps =
					new List<BuildStepAsync<T>>(),

				#endregion

				CurrentPool = null
			};

			return this;
		}

		#region Concrete pools

		public AsyncManagedPoolBuilder<T>
			AsyncStackManagedPool(
				AsyncStackManagedPoolFactory asyncStackManagedPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncStackManagedPoolFactory.BuildAsyncStackManagedPool(
							delegateContext.InitialValueAllocationCommand,
							delegateContext.AdditionalValueAllocationCommand,

							asyncContext,

							delegateContext.MetadataDescriptors,
							delegateContext.ResultFacadeAllocationCallback);
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T>
			AsyncQueueManagedPool(
				AsyncQueueManagedPoolFactory asyncQueueManagedPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncQueueManagedPoolFactory.BuildAsyncQueueManagedPool(
							delegateContext.InitialValueAllocationCommand,
							delegateContext.AdditionalValueAllocationCommand,

							asyncContext,

							delegateContext.MetadataDescriptors,
							delegateContext.ResultFacadeAllocationCallback);
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T>
			AsyncPackedArrayManagedPool(
				AsyncPackedArrayManagedPoolFactory asyncPackedArrayManagedPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncPackedArrayManagedPoolFactory.
							BuildAsyncPackedArrayManagedPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext,
	
								delegateContext.MetadataDescriptors,
								delegateContext.ResultFacadeAllocationCallback,
	
								true);
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T>
			AsyncLinkedListManagedPool(
				AsyncLinkedListManagedPoolFactory asyncLinkedListManagedPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncLinkedListManagedPoolFactory
							.BuildAsyncLinkedListManagedPool(
								delegateContext.InitialValueAllocationCommand,
								delegateContext.AdditionalValueAllocationCommand,

								asyncContext,
	
								delegateContext.MetadataDescriptors,
								delegateContext.ResultFacadeAllocationCallback);
				};

			return this;
		}

		#endregion

		#region Allocation command

		public AsyncManagedPoolBuilder<T> WithInitial(
			AllocationCommandDescriptor allocationCommand)
		{
			context.InitialAllocation = allocationCommand;

			return this;
		}

		public AsyncManagedPoolBuilder<T> WithAdditional(
			AllocationCommandDescriptor allocationCommand)
		{
			context.AdditionalAllocation = allocationCommand;

			return this;
		}

		public AsyncManagedPoolBuilder<T> WithActivatorAllocation()
		{
			context.ValueAllocationDelegate =
				async () =>
				{
					var result =
						AllocationFactory.ActivatorAllocationDelegate<T>();

					return result;
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T> WithActivatorAllocation<TValue>()
		{
			context.ValueAllocationDelegate =
				async () =>
				{
					var result =
						AllocationFactory.ActivatorAllocationDelegate<T, TValue>();

					return result;
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T> WithAllocationDelegate(
			Func<T> valueAllocationDelegate)
		{
			context.ValueAllocationDelegate =
				async () =>
				{
					var result = valueAllocationDelegate();

					return result;
				};

			return this;
		}

		public AsyncManagedPoolBuilder<T> WithAllocationTask(
			Func<Task<T>> valueAllocationTask)
		{
			context.ValueAllocationDelegate = valueAllocationTask;

			return this;
		}

		#endregion

		#region Callbacks

		public AsyncManagedPoolBuilder<T>
			PushNewElementsToPool(
				AsyncManagedObjectPoolAllocationCallbackFactory
					allocationCallbackFactory)
		{
			if (allocationCallbackFactory == null)
			{
				allocationCallbackFactory =
					new AsyncManagedObjectPoolAllocationCallbackFactory();
			}

			PushToAsyncManagedPoolCallback<T> pushCallback =
				allocationCallbackFactory.
					BuildPushToAsyncManagedPoolCallback<T>();

			context.FacadeAllocationCallbacks.Add(
				pushCallback);

			context.FinalBuildSteps.Add(
				async (context, asyncContext) =>
				{
					pushCallback.TargetPool =
						context.CurrentPool;
				});

			return this;
		}

		public AsyncManagedPoolBuilder<T>
			PushNewElementsToPoolWhenAvailable(
				AsyncManagedObjectPoolAllocationCallbackFactory
					allocationCallbackFactory)
		{
			if (allocationCallbackFactory == null)
			{
				allocationCallbackFactory =
					new AsyncManagedObjectPoolAllocationCallbackFactory();
			}

			PushToAsyncManagedPoolWhenAvailableCallback<T> pushCallback =
				allocationCallbackFactory.
					BuildPushToAsyncManagedPoolWhenAvailableCallback<T>();

			context.FacadeAllocationCallbacks.Add(
				pushCallback);

			context.FinalBuildSteps.Add(
				async (context, asyncContext) =>
				{
					await pushCallback.SetTargetPool(
						context.CurrentPool,
						
						asyncContext);
				});

			return this;
		}

		#endregion

		public async Task<IAsyncManagedPool<T>> Build(
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			BuildDependencies();

			foreach (var buildStep in context.FinalBuildSteps)
			{
				await buildStep?.Invoke(
					context,
					
					asyncContext);
			}

			context.ConcretePoolBuildStep?.Invoke(
				context,
				
				asyncContext);

			foreach (var buildStep in context.FinalBuildSteps)
			{
				buildStep?.Invoke(
					context,
					
					asyncContext);
			}

			var result = context.CurrentPool;

			Cleanup();

			return result;
		}

		public void BuildDependencies()
		{
			context.MetadataDescriptors = BuildMetadataDescriptors();

			context.ResultFacadeAllocationCallback = BuildFacadeAllocationCallback();

			context.ResultValueAllocationCallback = BuildValueAllocationCallback();

			context.InitialValueAllocationCommand =
				new AsyncAllocationCommand<T>(
					context.InitialAllocation,
					context.ValueAllocationDelegate,
					context.ResultValueAllocationCallback);

			context.AdditionalValueAllocationCommand =
				new AsyncAllocationCommand<T>(
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

		private IAsyncAllocationCallback<IAsyncPoolElementFacade<T>> 
			BuildFacadeAllocationCallback()
		{
			IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
				facadeAllocationCallback = null;

			if (context.FacadeAllocationCallbacks != null
				&& context.FacadeAllocationCallbacks.Count > 0)
			{
				facadeAllocationCallback =
					asyncAllocationCallbackFactory.
						BuildAsyncCompositeCallback<IAsyncPoolElementFacade<T>>(
							context.FacadeAllocationCallbacks);
			}

			return facadeAllocationCallback;
		}

		private IAsyncAllocationCallback<T> BuildValueAllocationCallback()
		{
			IAsyncAllocationCallback<T> valueAllocationCallback = null;

			if (context.ValueAllocationCallbacks != null
				&& context.ValueAllocationCallbacks.Count > 0)
			{
				valueAllocationCallback =
					asyncAllocationCallbackFactory.BuildAsyncCompositeCallback<T>(
						context.ValueAllocationCallbacks);
			}

			return valueAllocationCallback;
		}
	}
}