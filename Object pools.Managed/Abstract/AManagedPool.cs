using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.ObjectPools.Managed
{
	public abstract class AManagedPool<T>
		: IManagedPool<T>,
		  IManagedPoolInternal<T>,
		  IResizable,
		  IManagedResizable<T>,
		  ICleanuppable,
		  IDisposable
	{
		protected readonly IAllocationCommand<IPoolElementFacade<T>>
			facadeAllocationCommand;

		protected readonly IAllocationCommand<T> valueAllocationCommand;

		protected int capacity;

		public AManagedPool(
			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand)
		{
			this.facadeAllocationCommand = facadeAllocationCommand;

			this.valueAllocationCommand = valueAllocationCommand;
		}

		#region IManagedPool

		public virtual IPoolElementFacade<T> Pop()
		{
			IPoolElementFacade<T> result = PopFacade();

			//Validate values

			if (result.Status == EPoolElementStatus.UNINITIALIZED)
			{
				var newElement = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(newElement);

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

		public virtual IPoolElementFacade<T> Pop(
			IPoolPopArgument[] args)
		{
			return Pop();
		}

		public virtual void Push(
			IPoolElementFacade<T> instance)
		{
			// Validate values

			if (instance.Status != EPoolElementStatus.POPPED)
			{
				return;
			}

			//Update facade

			instance.Status = EPoolElementStatus.PUSHED;

			PushFacade(instance);
		}

		#endregion

		#region IManagedPoolInternal

		public IAllocationCommand<IPoolElementFacade<T>> FacadeAllocationCommand =>
			facadeAllocationCommand;

		public IAllocationCommand<T> ValueAllocationCommand
			=> valueAllocationCommand;

		public abstract IPoolElementFacade<T> PopFacade();

		public abstract void PushFacade(
			IPoolElementFacade<T> instance);

		#endregion

		#region IAllocationResizable

		public abstract void Resize();

		#endregion

		#region IManagedResizable

		public abstract void Resize(
			IAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized);

		#endregion

		#region ICleanUppable

		public abstract void Cleanup();

		#endregion

		#region IDisposable

		public abstract void Dispose();

		#endregion
	}
}