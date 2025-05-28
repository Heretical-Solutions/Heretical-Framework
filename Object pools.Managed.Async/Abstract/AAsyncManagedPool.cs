using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public abstract class AAsyncManagedPool<T>
		: IAsyncManagedPool<T>,
		  IAsyncManagedPoolInternal<T>,
		  IAsyncResizable,
		  IAsyncManagedResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		protected readonly IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
			facadeAllocationCommand;

		protected readonly IAsyncAllocationCommand<T> valueAllocationCommand;

		protected int capacity;

		public AAsyncManagedPool(
			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> 
				facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand)
		{
			this.facadeAllocationCommand = facadeAllocationCommand;

			this.valueAllocationCommand = valueAllocationCommand;
		}

		#region IAsyncManagedPool

		public virtual async Task<IAsyncPoolElementFacade<T>> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncPoolElementFacade<T> result = await PopFacade(
				asyncContext);

			//Validate values

			if (result.Status == EPoolElementStatus.UNINITIALIZED)
			{
				var newElement = await valueAllocationCommand.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				result.Value = newElement;
			}

			//Validate pool

			if (result.Pool == null)
			{
				result.Pool = this;
			}

			//Update facade

			result.Status = EPoolElementStatus.POPPED;

			return result;
		}

		public virtual async Task<IAsyncPoolElementFacade<T>> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await Pop(asyncContext);
		}

		public virtual async Task Push(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			// Validate values

			if (instance.Status != EPoolElementStatus.POPPED)
			{
				return;
			}

			//Update facade

			instance.Status = EPoolElementStatus.PUSHED;

			await PushFacade(
				instance,
				asyncContext);
		}

		#endregion

		#region IAsyncManagedPoolInternal

		public IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
			FacadeAllocationCommand =>
				facadeAllocationCommand;

		public IAsyncAllocationCommand<T> ValueAllocationCommand
			=> valueAllocationCommand;

		public abstract Task<IAsyncPoolElementFacade<T>> PopFacade(

			//Async tail
			AsyncExecutionContext asyncContext);

		public abstract Task PushFacade(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region IAsyncAllocationResizable

		public abstract Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region IAsyncManagedResizable

		public abstract Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region ICleanUppable

		public abstract void Cleanup();

		#endregion

		#region IDisposable

		public abstract void Dispose();

		#endregion
	}
}