using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async.Factories
{
	public class AsyncLinkedListPoolFactory
	{
		#region Build

		public async Task<AsyncLinkedListPool<T>> BuildAsyncLinkedListPool<T>(
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var linkedList = new LinkedList<T>();

			await PerformInitialAllocation<T>(
				linkedList,
				allocationDelegate,
				initialAmount,
				asyncContext);

			return new AsyncLinkedListPool<T>(
				linkedList,
				allocationDelegate);
		}

		private async Task PerformInitialAllocation<T>(
			LinkedList<T> linkedList,
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await allocationDelegate();

				linkedList.AddFirst(
					newElement);
			}
		}

		#endregion
	}
}