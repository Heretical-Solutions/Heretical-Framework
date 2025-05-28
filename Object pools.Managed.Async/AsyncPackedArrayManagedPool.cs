using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Collections;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Async.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncPackedArrayManagedPool<T>
		: AAsyncManagedPool<T>
	{
		protected readonly AsyncPackedArrayManagedPoolFactory
			asyncPackedArrayManagedPoolFactory;

		protected readonly bool validateValues;

		protected readonly ILogger logger;


		protected IAsyncPoolElementFacade<T>[] packedArray;

		protected int allocatedCount;

		public AsyncPackedArrayManagedPool(
			AsyncPackedArrayManagedPoolFactory
				asyncPackedArrayManagedPoolFactory,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			IAsyncPoolElementFacade<T>[] packedArray,

			ILogger logger,

			bool validateValues = true)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
		{
			this.asyncPackedArrayManagedPoolFactory =
				asyncPackedArrayManagedPoolFactory;


			this.packedArray = packedArray;

			this.validateValues = validateValues;

			this.logger = logger;


			allocatedCount = 0;
		}

		public override async Task<IAsyncPoolElementFacade<T>> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//Retrieve a pooled element

			IAsyncPoolElementFacade<T> result = default;

			//Resize the pool if necessary

			if (allocatedCount >= packedArray.Length)
			{
				await Resize(asyncContext);
			}

			int index = allocatedCount;

			result = packedArray[index];

			allocatedCount++;

			//Update metadata

			IAsyncPoolElementFacadeWithMetadata<T> resultWithMetadata =
				result as IAsyncPoolElementFacadeWithMetadata<T>;

			if (resultWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
			}

			//Update index

			var indexedFacade = resultWithMetadata as IIndexed;

			if (indexedFacade == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
			}

			indexedFacade.Index = index;

			//Validate values

			if (validateValues
				&& resultWithMetadata.Status == EPoolElementStatus.UNINITIALIZED)
			{
				var newElement = await valueAllocationCommand.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				result.Value = newElement;
			}

			//Validate pool

			if (result.Pool == null)
			{
				result.Pool = this;
			}

			//Update facade

			result.Status = EPoolElementStatus.POPPED;

			return result;
		}

		public override async Task Push(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int lastAllocatedItemIndex = allocatedCount - 1;

			if (lastAllocatedItemIndex < 0)
			{
				logger?.LogError(
					GetType(),
					$"ATTEMPT TO PUSH AN ITEM WHEN NO ITEMS ARE ALLOCATED");

				return;
			}

			// Validate values

			if (validateValues
				&& instance.Status != EPoolElementStatus.POPPED)
			{
				return;
			}

			IPoolElementFacadeWithMetadata<T> resultWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

			if (resultWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
			}

			//Get index

			var indexedFacade = resultWithMetadata as IIndexed;

			if (indexedFacade == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
			}

			int instanceIndex = indexedFacade.Index;


			if (instanceIndex == -1)
			{
				logger?.LogError(
					GetType(),
					$"ATTEMPT TO PUSH AN ITEM TO PACKED ARRAY IT DOES NOT BELONG TO");

				return;
			}

			if (instanceIndex > lastAllocatedItemIndex)
			{
				logger?.LogError(
					GetType(),
					$"ATTEMPT TO PUSH AN ALREADY PUSHED ITEM: {instanceIndex}");

				return;
			}

			if (instanceIndex != lastAllocatedItemIndex)
			{
				//Update last allocated element's index

				var lastAllocatedItem = packedArray[lastAllocatedItemIndex];

				IPoolElementFacadeWithMetadata<T> lastAllocatedItemWithMetadata =
					lastAllocatedItem as IPoolElementFacadeWithMetadata<T>;

				if (lastAllocatedItemWithMetadata == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
				}

				var lastAllocatedItemAsIndexable = lastAllocatedItemWithMetadata as IIndexed;

				if (lastAllocatedItemAsIndexable == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
				}

				lastAllocatedItemAsIndexable.Index = instanceIndex;

				//Swap pushed element and last allocated element

				var swap = packedArray[instanceIndex];

				packedArray[instanceIndex] = packedArray[lastAllocatedItemIndex];

				packedArray[lastAllocatedItemIndex] = swap;
			}

			//Update index

			indexedFacade.Index = -1;

			//Update facade

			instance.Status = EPoolElementStatus.PUSHED;

			allocatedCount--;
		}

		public override async Task<IAsyncPoolElementFacade<T>> PopFacade(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			throw new InvalidOperationException();
		}

		public override async Task PushFacade(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			throw new InvalidOperationException();
		}

		public override async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			packedArray = await asyncPackedArrayManagedPoolFactory.
				ResizeAsyncPackedArrayManagedPool<T>(
					packedArray,

					facadeAllocationCommand,
					valueAllocationCommand,

					true,
					
					asyncContext);
		}

		public override async Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			packedArray = await asyncPackedArrayManagedPoolFactory.
				ResizeAsyncPackedArrayManagedPool<T>(
					packedArray,

					facadeAllocationCommand,
					allocationCommand,

					newValuesAreInitialized,
					
					asyncContext);
		}

		#region IDynamicArray

		public int Capacity { get => packedArray.Length; }

		public int Count { get => allocatedCount; }

		public IAsyncPoolElementFacade<T> ElementAt(int index)
		{
			return packedArray[index];
		}

		public IAsyncPoolElementFacade<T> this[int index]
		{
			get
			{
				if (index >= allocatedCount || index < 0)
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID INDEX: {index} COUNT: {allocatedCount} CAPACITY: {Capacity}"));

				return packedArray[index];
			}
		}

		public IAsyncPoolElementFacade<T> Get(int index)
		{
			if (index >= allocatedCount || index < 0)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID INDEX: {index} COUNT: {allocatedCount} CAPACITY: {Capacity}"));

			return packedArray[index];
		}

		#endregion

		#region ICleanUppable

		public override void Cleanup()
		{
			foreach (var item in packedArray)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			allocatedCount = 0;
		}

		#endregion

		#region IDisposable

		public override void Dispose()
		{
			foreach (var item in packedArray)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			for (int i = 0; i < packedArray.Length; i++)
			{
				packedArray[i] = null;
			}

			allocatedCount = 0;
		}

		#endregion
	}
}