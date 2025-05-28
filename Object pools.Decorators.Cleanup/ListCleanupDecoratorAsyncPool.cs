using System.Collections;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
	public class ListCleanupDecoratorAsyncPool<T>
		: AAsyncDecoratorPool<T>
	{
		public ListCleanupDecoratorAsyncPool(
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
			var instanceAsList = instance as IList;

			instanceAsList?.Clear();
		}

		protected override async Task OnBeforePush(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsList = instance as IList;

			instanceAsList?.Clear();
		}
	}
}