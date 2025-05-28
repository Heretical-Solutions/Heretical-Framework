using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class PushToAsyncManagedPoolWhenAvailableCallback<T>
		: IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
	{
		private readonly List<IAsyncPoolElementFacade<T>> elementsToPush;

		private IAsyncManagedPool<T> targetPool;

		public PushToAsyncManagedPoolWhenAvailableCallback(
			List<IAsyncPoolElementFacade<T>> elementsToPush,
			IAsyncManagedPool<T> targetPool = null)
		{
			this.elementsToPush = elementsToPush;

			this.targetPool = targetPool;
		}

		public IAsyncManagedPool<T> TargetPool
		{
			get => targetPool;
		}

		public async Task SetTargetPool(
			IAsyncManagedPool<T> value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			targetPool = value;

			if (targetPool != null)
			{
				foreach (var element in elementsToPush)
				{
					element.Pool = targetPool;

					await targetPool.Push(
						element,
						asyncContext);
				}

				elementsToPush.Clear();
			}
		}

		public async Task OnAllocated(
			IAsyncPoolElementFacade<T> currentElementFacade,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (targetPool != null)
			{
				currentElementFacade.Pool = targetPool;

				await targetPool.Push(
					currentElementFacade,
					asyncContext);
			}
			else
			{
				elementsToPush.Add(
					currentElementFacade);
			}
		}
	}
}