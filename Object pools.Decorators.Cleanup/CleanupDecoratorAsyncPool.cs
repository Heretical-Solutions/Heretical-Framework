using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
	public class CleanupDecoratorAsyncPool<T>
		: AAsyncDecoratorPool<T>
	{
		public CleanupDecoratorAsyncPool(
			IAsyncPool<T> innerPool)
			: base(innerPool)
		{
		}

		protected override async Task OnAfterPop(
			T instance,
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsCleanUppable = instance as ICleanuppable;

			instanceAsCleanUppable?.Cleanup();
		}

		protected override async Task OnBeforePush(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsCleanUppable = instance as ICleanuppable;

			instanceAsCleanUppable?.Cleanup();
		}
	}
}