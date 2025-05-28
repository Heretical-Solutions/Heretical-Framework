using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools.Managed;
using HereticalSolutions.ObjectPools.Managed.Async;

namespace HereticalSolutions.ObjectPools.Decorators.Concurrent
{
	public class ConcurrentDecoratorAsyncManagedPool<T>
		: AAsyncDecoratorManagedPool<T>
	{
		private readonly object lockObject;

		private bool isPopping;

		private bool isPushing;

		public ConcurrentDecoratorAsyncManagedPool(
			IAsyncManagedPool<T> innerPool,
			object lockObject)
			: base(innerPool)
		{
			this.lockObject = lockObject;
		}

		#region IPool

		public override async Task<IAsyncPoolElementFacade<T>> Pop(

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

			IAsyncPoolElementFacade<T> result = await innerPool.Pop(
				asyncContext);

			lock (lockObject)
			{
				isPopping = false;
			}

			return result;
		}

		public override async Task<IAsyncPoolElementFacade<T>> Pop(
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

			IAsyncPoolElementFacade<T> result = await innerPool.Pop(
				args,
				asyncContext);

			lock (lockObject)
			{
				isPopping = false;
			}

			return result;
		}

		public override async Task Push(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//During the Pop, the pool may resize if it runs out of instances
			//While the pool is resizing, it is a good idea not to disturb it
			//with other Pop or Push operations. Howerer, during the resizing
			//the allocation callback may fire that would ask the instnace to
			//be pooled as a kind of 'dry run' to push it through decorator
			//pools and execute their logic without pushing the instance in the
			//end. So now we have to prevent pushes and pops during the potential
			//resize while allowing the allocation callbacks to push new values.
			//To make it happen we need a way to identify whether the value pushed
			//is provided by the allocation callback or not. We do so by checking
			//the instance status. If the instance is not in the POPPED state,
			//we push it unless some other instance is being pushed
			bool valueIsInitialized = instance.Status == EPoolElementStatus.POPPED;

			bool isPoppingClosure = false;

			bool isPushingClosure = false;

			lock (lockObject)
			{
				isPoppingClosure = isPopping;

				isPushingClosure = isPushing;
			}

			while ((isPoppingClosure && valueIsInitialized)
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