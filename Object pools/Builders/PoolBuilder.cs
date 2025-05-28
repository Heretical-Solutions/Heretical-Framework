using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.ObjectPools.Factories;

namespace HereticalSolutions.ObjectPools.Builders
{
	public class PoolBuilder<T>
		: ABuilder<PoolBuilderContext<T>>
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

		public PoolBuilder(
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

		public PoolBuilder(
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

		public PoolBuilder<T>
			New()
		{
			context = new PoolBuilderContext<T>
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

				#region Building

				InitialBuildSteps =
					new List<Action<PoolBuilderContext<T>>>(),

				ConcretePoolBuildStep = null,

				FinalBuildSteps =
					new List<Action<PoolBuilderContext<T>>>(),

				#endregion

				CurrentPool = null
			};

			return this;
		}

		#region Concrete pools

		public PoolBuilder<T>
			StackPool(
				StackPoolFactory stackPoolFactory)
		{
			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						stackPoolFactory.BuildStackPool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor.CountInitialAllocationAmount());
				};

			return this;
		}

		public PoolBuilder<T>
			QueuePool(
				QueuePoolFactory queuePoolFactory)
		{
			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						queuePoolFactory.BuildQueuePool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor.CountInitialAllocationAmount());
				};

			return this;
		}

		public PoolBuilder<T>
			PackedArrayPool(
				PackedArrayPoolFactory packedArrayPoolFactory)
		{
			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						packedArrayPoolFactory.BuildPackedArrayPool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor.CountInitialAllocationAmount());
				};

			return this;
		}

		public PoolBuilder<T>
			LinkedListPool(
				LinkedListPoolFactory linkedListPoolFactory)
		{
			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					delegateContext.CurrentPool =
						linkedListPoolFactory.BuildLinkedListPool(
							delegateContext
								.InitialValueAllocationCommand
								.AllocationDelegate,
							delegateContext
								.InitialValueAllocationCommand
								.Descriptor.CountInitialAllocationAmount());
				};

			return this;
		}

		#endregion

		#region Allocation command

		public PoolBuilder<T> WithInitial(
			AllocationCommandDescriptor allocationCommand)
		{
			context.InitialAllocation = allocationCommand;

			return this;
		}

		public PoolBuilder<T> WithAdditional(
			AllocationCommandDescriptor allocationCommand)
		{
			context.AdditionalAllocation = allocationCommand;

			return this;
		}

		public PoolBuilder<T> WithActivatorAllocation()
		{
			context.ValueAllocationDelegate =
				AllocationFactory.ActivatorAllocationDelegate<T>;

			return this;
		}

		public PoolBuilder<T> WithActivatorAllocation<TValue>()
		{
			context.ValueAllocationDelegate =
				AllocationFactory.ActivatorAllocationDelegate<T, TValue>;

			return this;
		}

		public PoolBuilder<T> WithAllocationDelegate(
			Func<T> valueAllocationDelegate)
		{
			context.ValueAllocationDelegate = valueAllocationDelegate;

			return this;
		}

		#endregion

		public IPool<T> Build()
		{
			BuildDependencies();

			foreach (var buildStep in context.FinalBuildSteps)
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
			context.ResultValueAllocationCallback = BuildValueAllocationCallback();

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

		private IAllocationCallback<T> BuildValueAllocationCallback()
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