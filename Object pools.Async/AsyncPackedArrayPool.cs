using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async
{
	public class AsyncPackedArrayPool<T>
		: IAsyncPool<T>,
		  IAsyncResizable,
		  IAsyncNumericalResizable
	{
		private readonly Func<Task<T>> allocationDelegate;

		private T[] packedArray;

		private int allocatedCount;

		public AsyncPackedArrayPool(
			T[] packedArray,
			Func<Task<T>> allocationDelegate)
		{
			this.packedArray = packedArray;

			this.allocationDelegate = allocationDelegate;

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
				throw new Exception(
					$"ATTEMPT TO PUSH AN ITEM WHEN NO ITEMS ARE ALLOCATED");
			}

			int instanceIndex = Array.IndexOf(
				packedArray,
				instance);

			if (instanceIndex == -1)
			{
				throw new Exception(
					$"ATTEMPT TO PUSH AN ITEM TO PACKED ARRAY IT DOES NOT BELONG TO");
			}

			if (instanceIndex > lastAllocatedItemIndex)
			{
				throw new Exception(
					$"ATTEMPT TO PUSH AN ALREADY PUSHED ITEM: {instanceIndex}");
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

		#region IAsyncResizable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await Resize(
				packedArray.Length,
				asyncContext);
		}

		#endregion

		#region IAsyncNumericalResizable

		public async Task Resize(
			int additionalAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var currentAmount = packedArray.Length;

			var newPackedArray = new T[currentAmount + additionalAmount];

			Array.Copy(
				packedArray,
				newPackedArray,
				currentAmount);

			for (int i = currentAmount; i < packedArray.Length; i++)
			{
				packedArray[i] = await allocationDelegate();
			}
		}

		#endregion
	}
}