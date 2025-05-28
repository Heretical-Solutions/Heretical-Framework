using System.Collections;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
	public class ListCleanupDecoratorAsyncManagedPool<T>
		: AAsyncDecoratorManagedPool<T>
	{
		public ListCleanupDecoratorAsyncManagedPool(
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
			var instanceAsList = instance as IList;

			instanceAsList?.Clear();
		}

		protected override async Task OnBeforePush(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsList = instance as IList;

			instanceAsList?.Clear();
		}
	}
}