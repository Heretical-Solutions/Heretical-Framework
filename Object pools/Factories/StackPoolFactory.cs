using System;
using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools.Factories
{
	public class StackPoolFactory
	{
		#region Build

		public StackPool<T> BuildStackPool<T>(
			Func<T> allocationDelegate,
			int initialAmount)
		{
			var stack = new Stack<T>();

			PerformInitialAllocation<T>(
				stack,
				allocationDelegate,
				initialAmount);

			return new StackPool<T>(
				stack,
				allocationDelegate);
		}

		private void PerformInitialAllocation<T>(
			Stack<T> stack,
			Func<T> allocationDelegate,
			int initialAmount)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = allocationDelegate();

				stack.Push(
					newElement);
			}
		}

		#endregion
	}
}