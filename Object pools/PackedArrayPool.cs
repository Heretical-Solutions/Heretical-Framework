using System;

namespace HereticalSolutions.ObjectPools
{
	public class PackedArrayPool<T>
		: IPool<T>,
		  IResizable,
		  INumericalResizable
	{
		private readonly Func<T> allocationDelegate;

		private T[] packedArray;

		private int allocatedCount;

		public PackedArrayPool(
			T[] packedArray,
			Func<T> allocationDelegate)
		{
			this.packedArray = packedArray;

			this.allocationDelegate = allocationDelegate;

			allocatedCount = 0;
		}

		#region IPool

		public T Pop()
		{
			T result = default;

			if (allocatedCount >= packedArray.Length)
			{
				Resize();
			}

			result = packedArray[allocatedCount];

			allocatedCount++;

			return result;
		}

		public T Pop(
			IPoolPopArgument[] args)
		{
			return Pop();
		}

		public void Push(
			T instance)
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

		#region IResizable

		public void Resize()
		{
			Resize(packedArray.Length);
		}

		#endregion

		#region INumericalResizable

		public void Resize(
			int additionalAmount)
		{
			var currentAmount = packedArray.Length;

			var newPackedArray = new T[currentAmount + additionalAmount];

			Array.Copy(
				packedArray,
				newPackedArray,
				currentAmount);

			for (int i = currentAmount; i < packedArray.Length; i++)
			{
				packedArray[i] = allocationDelegate();
			}
		}

		#endregion
	}
}