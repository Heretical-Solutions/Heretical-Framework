using System;

using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Decorators
{
	public class NonAllocPoolWithRuntimeTimer<T> : ANonAllocDecoratorPool<T>
	{
		private readonly ISynchronizationProvider provider;

		public NonAllocPoolWithRuntimeTimer(
			INonAllocDecoratedPool<T> innerPool,
			ISynchronizationProvider provider)
			: base(innerPool)
		{
			this.provider = provider;
		}
		
		protected override void OnAfterPop(
			IPoolElement<T> instance,
			IPoolDecoratorArgument[] args)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception("[NonAllocPoolWithRuntimeTimer] INVALID INSTANCE");

			provider.Subscribe(instance.Metadata.Get<IContainsRuntimeTimer>().Subscription);
		}

		protected override void OnBeforePush(IPoolElement<T> instance)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception("[NonAllocPoolWithRuntimeTimer] INVALID INSTANCE");

			provider.Unsubscribe(instance.Metadata.Get<IContainsRuntimeTimer>().Subscription);
		}
	}
}