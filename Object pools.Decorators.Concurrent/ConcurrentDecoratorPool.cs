namespace HereticalSolutions.ObjectPools.Decorators.Concurrent
{
	public class ConcurrentDecoratorPool<T>
		: ADecoratorPool<T>
	{
		private readonly object lockObject;

		public ConcurrentDecoratorPool(
			IPool<T> innerPool,
			object lockObject)
			: base(innerPool)
		{
			this.lockObject = lockObject;
		}

		#region IPool

		public override T Pop()
		{
			lock (lockObject)
			{
				return base.Pop();
			}
		}

		public override T Pop(
			IPoolPopArgument[] args)
		{
			lock (lockObject)
			{
				return base.Pop(args);
			}
		}

		public override void Push(
			T instance)
		{
			lock (lockObject)
			{
				base.Push(instance);
			}
		}

		#endregion
	}
}