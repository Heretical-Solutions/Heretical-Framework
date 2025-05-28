using HereticalSolutions.ObjectPools.Async;
using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories
{
	public class CleanupDecoratorAsyncPoolFactory
	{
		public CleanupDecoratorAsyncPool<T>
			BuildCleanupDecoratorAsyncPool<T>(
				IAsyncPool<T> innerPool)
		{
			return new CleanupDecoratorAsyncPool<T>(
				innerPool);
		}

		public ListCleanupDecoratorAsyncPool<T>
			BuildListCleanupDecoratorAsyncPool<T>(
				IAsyncPool<T> innerPool)
		{
			return new ListCleanupDecoratorAsyncPool<T>(
				innerPool);
		}

		public CleanupDecoratorAsyncManagedPool<T> 
			BuildCleanupDecoratorAsyncManagedPool<T>(
				IAsyncManagedPool<T> innerPool)
		{
			return new CleanupDecoratorAsyncManagedPool<T>(
				innerPool);
		}

		public ListCleanupDecoratorAsyncManagedPool<T> 
			BuildListCleanupDecoratorAsyncManagedPool<T>(
				IAsyncManagedPool<T> innerPool)
		{
			return new ListCleanupDecoratorAsyncManagedPool<T>(
				innerPool);
		}
	}
}