using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent.Factories
{
	public class ConcurrentDecoratorPoolFactory
	{
		public ConcurrentDecoratorPool<T> BuildConcurrentDecoratorPool<T>(
			IPool<T> innerPool)
		{
			return new ConcurrentDecoratorPool<T>(
				innerPool,
				new object());
		}

		public ConcurrentDecoratorManagedPool<T> BuildConcurrentDecoratorManagedPool<T>(
			IManagedPool<T> innerPool)
		{
			return new ConcurrentDecoratorManagedPool<T>(
				innerPool,
				new object());
		}
	}
}