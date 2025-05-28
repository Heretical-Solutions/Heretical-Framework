using System;
using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools.Factories
{
	public class QueuePoolFactory
	{
		#region Build

		public QueuePool<T> BuildQueuePool<T>(
			Func<T> allocationDelegate,
			int initialAmount)
		{
			var queue = new Queue<T>();

			PerformInitialAllocation<T>(
				queue,
				allocationDelegate,
				initialAmount);

			return new QueuePool<T>(
				queue,
				allocationDelegate);
		}

		private void PerformInitialAllocation<T>(
			Queue<T> queue,
			Func<T> allocationDelegate,
			int initialAmount)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = allocationDelegate();

				queue.Enqueue(
					newElement);
			}
		}

		#endregion
	}
}