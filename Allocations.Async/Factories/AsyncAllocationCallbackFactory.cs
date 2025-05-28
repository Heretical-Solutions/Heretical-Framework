using System.Collections.Generic;

namespace HereticalSolutions.Allocations.Async.Factories
{
	public class AsyncAllocationCallbackFactory
	{
		public AsyncCompositeAllocationCallback<T>
			BuildAsyncCompositeCallback<T>(
				IAsyncAllocationCallback<T>[] callbacks)
		{
			List<IAsyncAllocationCallback<T>> callbacksList =
				new List<IAsyncAllocationCallback<T>>(callbacks);

			return new AsyncCompositeAllocationCallback<T>(callbacksList);
		}

		public AsyncCompositeAllocationCallback<T>
			BuildAsyncCompositeCallback<T>(
				List<IAsyncAllocationCallback<T>> callbacks)
		{
			return new AsyncCompositeAllocationCallback<T>(callbacks);
		}
	}
}