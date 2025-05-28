using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async.Factories
{
	public class AsyncStackPoolFactory
	{
		#region Build

		public async Task<AsyncStackPool<T>> BuildAsyncStackPool<T>(
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var stack = new Stack<T>();

			await PerformInitialAllocation<T>(
				stack,
				allocationDelegate,
				initialAmount,
				asyncContext);

			return new AsyncStackPool<T>(
				stack,
				allocationDelegate);
		}

		private async Task PerformInitialAllocation<T>(
			Stack<T> stack,
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await allocationDelegate();

				stack.Push(
					newElement);
			}
		}

		#endregion
	}
}