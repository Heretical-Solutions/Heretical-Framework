using HereticalSolutions.ObjectPools.Async;
using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories
{
	public class ConcurrentDecoratorAsyncPoolFactory
	{
		public ConcurrentDecoratorAsyncPool<T>
			BuildConcurrentDecoratorAsyncPool<T>(
				IAsyncPool<T> innerPool)
		{
			return new ConcurrentDecoratorAsyncPool<T>(
				innerPool,
				new object());
		}

		public ConcurrentDecoratorAsyncManagedPool<T> 
			BuildConcurrentDecoratorAsyncManagedPool<T>(
				IAsyncManagedPool<T> innerPool)
		{
			return new ConcurrentDecoratorAsyncManagedPool<T>(
				innerPool,
				new object());
		}
	}
}