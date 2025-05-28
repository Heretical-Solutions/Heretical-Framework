using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Allocations.Async;
using HereticalSolutions.Allocations.Async.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.ObjectPools.Async.Factories;

namespace HereticalSolutions.ObjectPools.Async.Builders
{
	public class AsyncPoolBuilder<T>
		: ABuilder<AsyncPoolBuilderContext<T>>
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

		public AsyncPoolBuilder(
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

		public AsyncPoolBuilder(
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

		public AsyncPoolBuilder<T>
			New()
		{
			context = new AsyncPoolBuilderContext<T>
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

		public AsyncPoolBuilder<T>
			AsyncStackPool(
				AsyncStackPoolFactory asyncStackPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncStackPoolFactory.BuildAsyncStackPool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor
								.CountInitialAllocationAmount(),

							asyncContext);
				};

			return this;
		}

		public AsyncPoolBuilder<T>
			AsyncQueuePool(
				AsyncQueuePoolFactory asyncQueuePoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncQueuePoolFactory.BuildAsyncQueuePool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor
								.CountInitialAllocationAmount(),

							asyncContext);
				};

			return this;
		}

		public AsyncPoolBuilder<T>
			AsyncPackedArrayPool(
				AsyncPackedArrayPoolFactory asyncPackedArrayPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncPackedArrayPoolFactory.
							BuildAsyncPackedArrayPool(
								delegateContext
									.InitialValueAllocationCommand
									.AllocationDelegate,
								delegateContext
									.InitialValueAllocationCommand
									.Descriptor
									.CountInitialAllocationAmount(),

								asyncContext);
				};

			return this;
		}

		public AsyncPoolBuilder<T>
			AsyncLinkedListPool(
				AsyncLinkedListPoolFactory asyncLinkedListPoolFactory)
		{
			context.ConcretePoolBuildStep =
				async (delegateContext, asyncContext) =>
				{
					delegateContext.CurrentPool =
						await asyncLinkedListPoolFactory
							.BuildAsyncLinkedListPool(
									delegateContext
									.InitialValueAllocationCommand
									.AllocationDelegate,
								delegateContext
									.InitialValueAllocationCommand
									.Descriptor
									.CountInitialAllocationAmount(),

								asyncContext);
				};

			return this;
		}

		#endregion

		#region Allocation command

		public AsyncPoolBuilder<T> WithInitial(
			AllocationCommandDescriptor allocationCommand)
		{
			context.InitialAllocation = allocationCommand;

			return this;
		}

		public AsyncPoolBuilder<T> WithAdditional(
			AllocationCommandDescriptor allocationCommand)
		{
			context.AdditionalAllocation = allocationCommand;

			return this;
		}

		public AsyncPoolBuilder<T> WithActivatorAllocation()
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

		public AsyncPoolBuilder<T> WithActivatorAllocation<TValue>()
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

		public AsyncPoolBuilder<T> WithAllocationDelegate(
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

		public AsyncPoolBuilder<T> WithAllocationTask(
			Func<Task<T>> valueAllocationTask)
		{
			context.ValueAllocationDelegate = valueAllocationTask;

			return this;
		}

		#endregion

		public async Task<IAsyncPool<T>> Build(
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