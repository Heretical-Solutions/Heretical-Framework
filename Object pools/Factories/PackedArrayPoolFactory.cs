using System;

namespace HereticalSolutions.ObjectPools.Factories
{
	public class PackedArrayPoolFactory
	{
		#region Build

		public PackedArrayPool<T> BuildPackedArrayPool<T>(
			Func<T> allocationDelegate,
			int initialAmount)
		{
			T[] contents = new T[initialAmount];

			PerformInitialAllocation<T>(
				contents,
				allocationDelegate,
				initialAmount);

			return new PackedArrayPool<T>(
				contents,
				allocationDelegate);
		}

		private void PerformInitialAllocation<T>(
			T[] contents,
			Func<T> allocationDelegate,
			int initialAmount)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = allocationDelegate();

				contents[i] = newElement;
			}
		}

		#endregion
	}
}