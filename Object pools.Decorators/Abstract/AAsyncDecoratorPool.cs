using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Decorators
{
	public abstract class AAsyncDecoratorPool<T>
		: IAsyncPool<T>,
		  ICleanuppable,
		  IDisposable
	{
		protected readonly IAsyncPool<T> innerPool;

		public IAsyncPool<T> InnerPool => innerPool;

		public AAsyncDecoratorPool(
			IAsyncPool<T> innerPool)
		{
			this.innerPool = innerPool;
		}

		#region IPool

		public virtual async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await OnBeforePop(
				null,
				asyncContext);

			T result = await innerPool.Pop(
				asyncContext);

			await OnAfterPop(
				result,
				null,
				asyncContext);

			return result;
		}

		public virtual async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await OnBeforePop(
				args,
				asyncContext);

			T result = await innerPool.Pop(
				args,
				asyncContext);

			await OnAfterPop(
				result,
				args,
				asyncContext);

			return result;
		}

		public virtual async Task Push(
			T instance,

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
			T instance,
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		protected virtual async Task OnBeforePush(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}

		protected virtual async Task OnAfterPush(
			T instance,

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