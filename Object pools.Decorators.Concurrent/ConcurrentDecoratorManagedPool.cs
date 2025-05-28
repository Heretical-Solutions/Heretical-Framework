using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent
{
	public class ConcurrentDecoratorManagedPool<T>
		: ADecoratorManagedPool<T>
	{
		private readonly object lockObject;

		public ConcurrentDecoratorManagedPool(
			IManagedPool<T> innerPool,
			object lockObject)
			: base(innerPool)
		{
			this.lockObject = lockObject;
		}

		#region IPool

		public override IPoolElementFacade<T> Pop()
		{
			lock (lockObject)
			{
				return base.Pop();
			}
		}

		public override IPoolElementFacade<T> Pop(
			IPoolPopArgument[] args)
		{
			lock (lockObject)
			{
				return base.Pop(args);
			}
		}

		public override void Push(
			IPoolElementFacade<T> instance)
		{
			lock (lockObject)
			{
				base.Push(instance);
			}
		}

		#endregion
	}
}