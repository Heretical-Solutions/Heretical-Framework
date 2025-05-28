using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent
{
	public class ConcurrentDecoratorAsyncPool<T>
		: AAsyncDecoratorPool<T>
	{
		private readonly object lockObject;

		private bool isPopping;

		private bool isPushing;

		public ConcurrentDecoratorAsyncPool(
			IAsyncPool<T> innerPool,
			object lockObject)
			: base(innerPool)
		{
			this.lockObject = lockObject;
		}

		#region IPool

		public override async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool isPoppingClosure = false;

			bool isPushingClosure = false;

			lock (lockObject)
			{
				isPoppingClosure = isPopping;

				isPushingClosure = isPushing;
			}

			while (isPoppingClosure || isPushingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isPoppingClosure = isPopping;

					isPushingClosure = isPushing;
				}
			}

			lock (lockObject)
			{
				isPopping = true;
			}

			T result = await innerPool.Pop(
				asyncContext);

			lock (lockObject)
			{
				isPopping = false;
			}

			return result;
		}

		public override async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool isPoppingClosure = false;

			bool isPushingClosure = false;

			lock (lockObject)
			{
				isPoppingClosure = isPopping;

				isPushingClosure = isPushing;
			}

			while (isPoppingClosure || isPushingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isPoppingClosure = isPopping;

					isPushingClosure = isPushing;
				}
			}

			lock (lockObject)
			{
				isPopping = true;
			}

			T result = await innerPool.Pop(
				args,
				asyncContext);

			lock (lockObject)
			{
				isPopping = false;
			}

			return result;
		}

		public override async Task Push(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool isPoppingClosure = false;

			bool isPushingClosure = false;

			lock (lockObject)
			{
				isPoppingClosure = isPopping;

				isPushingClosure = isPushing;
			}

			while (isPoppingClosure
				|| isPushingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isPoppingClosure = isPopping;

					isPushingClosure = isPushing;
				}
			}

			lock (lockObject)
			{
				isPushing = true;
			}

			await innerPool.Push(
				instance,

				asyncContext);

			lock (lockObject)
			{
				isPushing = false;
			}
		}

		#endregion
	}
}