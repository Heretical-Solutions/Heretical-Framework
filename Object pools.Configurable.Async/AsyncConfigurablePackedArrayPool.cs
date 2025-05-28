using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Async;
using HereticalSolutions.ObjectPools.Configurable.Async.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Async
{
	public class AsyncConfigurablePackedArrayPool<T>
		: IAsyncPool<T>,
		  IAsyncResizable,
		  IAsyncAllocationCommandResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IAsyncAllocationCommand<T> allocationCommand;

		private readonly AsyncConfigurablePackedArrayPoolFactory factory;

		private readonly ILogger logger;

		private T[] packedArray;

		private int allocatedCount;

		public AsyncConfigurablePackedArrayPool(
			T[] packedArray,
			IAsyncAllocationCommand<T> allocationCommand,
			AsyncConfigurablePackedArrayPoolFactory factory,
			ILogger logger)
		{
			this.packedArray = packedArray;

			this.allocationCommand = allocationCommand;

			this.factory = factory;

			this.logger = logger;

			allocatedCount = 0;
		}

		#region IPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T result = default;

			if (allocatedCount >= packedArray.Length)
			{
				await Resize(asyncContext);
			}

			result = packedArray[allocatedCount];

			allocatedCount++;

			return result;
		}

		public async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await Pop(asyncContext);
		}

		public async Task Push(
			T instance,

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

			int instanceIndex = Array.IndexOf(
				packedArray,
				instance);

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
				//Swap pushed element and last allocated element

				var swap = packedArray[instanceIndex];

				packedArray[instanceIndex] = packedArray[lastAllocatedItemIndex];

				packedArray[lastAllocatedItemIndex] = swap;
			}

			allocatedCount--;
		}

		#endregion

		#region IAsyncAllocationResizable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			packedArray = await factory.ResizeAsyncConfigurablePackedArrayPool(
				packedArray,
				allocationCommand,
				asyncContext);
		}

		#endregion

		#region IAsyncAllocationCommandResizable

		public async Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			packedArray = await factory.ResizeAsyncConfigurablePackedArrayPool(
				packedArray,
				allocationCommand,
				asyncContext);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var item in packedArray)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			allocatedCount = 0;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var item in packedArray)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			for (int i = 0; i < packedArray.Length; i++)
			{
				packedArray[i] = default;
			}

			allocatedCount = 0;
		}

		#endregion
	}
}