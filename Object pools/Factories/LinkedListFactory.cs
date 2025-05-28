using System;
using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools.Factories
{
	public class LinkedListPoolFactory
	{
		#region Build

		public LinkedListPool<T> BuildLinkedListPool<T>(
			Func<T> allocationDelegate,
			int initialAmount)
		{
			var linkedList = new LinkedList<T>();

			PerformInitialAllocation<T>(
				linkedList,
				allocationDelegate,
				initialAmount);

			return new LinkedListPool<T>(
				linkedList,
				allocationDelegate);
		}

		private void PerformInitialAllocation<T>(
			LinkedList<T> linkedList,
			Func<T> allocationDelegate,
			int initialAmount)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = allocationDelegate();

				linkedList.AddFirst(
					newElement);
			}
		}

		#endregion
	}
}