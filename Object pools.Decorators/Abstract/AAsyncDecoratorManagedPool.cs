using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators
{
	public abstract class AAsyncDecoratorManagedPool<T>
		: IAsyncManagedPool<T>,
		  ICleanuppable,
		  IDisposable
	{
		protected readonly IAsyncManagedPool<T> innerPool;

		public IAsyncManagedPool<T> InnerPool => innerPool;

		public AAsyncDecoratorManagedPool(
			IAsyncManagedPool<T> innerPool)
		{
			this.innerPool = innerPool;
		}

		#region IManagedPool

		public virtual async Task<IAsyncPoolElementFacade<T>> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await OnBeforePop(
				null,
				asyncContext);

			IAsyncPoolElementFacade<T> result = await innerPool.Pop(
				asyncContext);

			await OnAfterPop(
				result,
				null,
				asyncContext);

			return result;
		}

		public virtual async Task<IAsyncPoolElementFacade<T>> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await OnBeforePop(
				args,
				asyncContext);

			IAsyncPoolElementFacade<T> result = await innerPool.Pop(
				args,
				asyncContext);

			await OnAfterPop(
				result,
				args,
				asyncContext);

			return result;
		}

		public virtual async Task Push(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await OnBeforePush(
				instance,
				asyncContext);

			await innerPool.Push(
				instance,
				asyncContext);

			await OnAfterPush(
				instance,
				asyncContext);
		}

		#endregion

		protected virtual async Task OnBeforePop(
			IPoolPopArgument[] args,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		protected virtual async Task OnAfterPop(
			IAsyncPoolElementFacade<T> instance,
			IPoolPopArgument[] args,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		protected virtual async Task OnBeforePush(
			IAsyncPoolElementFacade<T> instance,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		protected virtual async Task OnAfterPush(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		#region ICleanUppable

		public void Cleanup()
		{
			if (innerPool is ICleanuppable)
				(innerPool as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (innerPool is IDisposable)
				(innerPool as IDisposable).Dispose();
		}

		#endregion
	}
}