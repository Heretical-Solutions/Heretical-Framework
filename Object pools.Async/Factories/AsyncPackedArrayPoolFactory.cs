using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async.Factories
{
	public class AsyncPackedArrayPoolFactory
	{
		#region Build

		public async Task<AsyncPackedArrayPool<T>> BuildAsyncPackedArrayPool<T>(
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T[] contents = new T[initialAmount];

			await PerformInitialAllocation<T>(
				contents,
				allocationDelegate,
				initialAmount,
				asyncContext);

			return new AsyncPackedArrayPool<T>(
				contents,
				allocationDelegate);
		}

		private async Task PerformInitialAllocation<T>(
			T[] contents,
			Func<Task<T>> allocationDelegate,
			int initialAmount,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await allocationDelegate();

				contents[i] = newElement;
			}
		}

		#endregion
	}
}