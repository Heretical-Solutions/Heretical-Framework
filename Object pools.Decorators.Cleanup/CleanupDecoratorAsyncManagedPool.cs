using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
	public class CleanupDecoratorAsyncManagedPool<T>
		: AAsyncDecoratorManagedPool<T>
	{
		public CleanupDecoratorAsyncManagedPool(
			IAsyncManagedPool<T> innerPool)
			: base(innerPool)
		{
		}

		protected override async Task OnAfterPop(
			IAsyncPoolElementFacade<T> instance,
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsCleanUppable = instance as ICleanuppable;

			instanceAsCleanUppable?.Cleanup();
		}

		protected override async Task OnBeforePush(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsCleanUppable = instance as ICleanuppable;

			instanceAsCleanUppable?.Cleanup();
		}
	}
}