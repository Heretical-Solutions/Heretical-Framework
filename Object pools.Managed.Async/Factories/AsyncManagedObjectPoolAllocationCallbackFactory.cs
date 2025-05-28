using System.Collections.Generic;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncManagedObjectPoolAllocationCallbackFactory
	{
		public PushToAsyncManagedPoolCallback<T>
			BuildPushToAsyncManagedPoolCallback<T>(
				IAsyncManagedPool<T> root = null)
		{
			return new PushToAsyncManagedPoolCallback<T>
			{
				TargetPool = root
			};
		}

		public PushToAsyncManagedPoolWhenAvailableCallback<T>
			BuildPushToAsyncManagedPoolWhenAvailableCallback<T>(
				IAsyncManagedPool<T> root = null)
		{
			return new PushToAsyncManagedPoolWhenAvailableCallback<T>(
				new List<IAsyncPoolElementFacade<T>>(),
				root);
		}
	}
}