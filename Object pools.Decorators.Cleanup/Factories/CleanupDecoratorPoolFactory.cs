using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup.Factories
{
	public class CleanupDecoratorPoolFactory
	{
		public CleanupDecoratorPool<T> BuildCleanupDecoratorPool<T>(
			IPool<T> innerPool)
		{
			return new CleanupDecoratorPool<T>(
				innerPool);
		}

		public ListCleanupDecoratorPool<T> BuildListCleanupDecoratorPool<T>(
			IPool<T> innerPool)
		{
			return new ListCleanupDecoratorPool<T>(
				innerPool);
		}

		public CleanupDecoratorManagedPool<T> BuildCleanupDecoratorManagedPool<T>(
			IManagedPool<T> innerPool)
		{
			return new CleanupDecoratorManagedPool<T>(
				innerPool);
		}

		public ListCleanupDecoratorManagedPool<T> BuildListCleanupDecoratorManagedPool<T>(
			IManagedPool<T> innerPool)
		{
			return new ListCleanupDecoratorManagedPool<T>(
				innerPool);
		}
	}
}