using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncPoolElementFacade<T>
		: IAsyncPoolElementFacadeWithMetadata<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IStronglyTypedMetadata metadata;

		private EPoolElementStatus status;

		private IAsyncManagedPool<T> pool;

		public AsyncPoolElementFacade(
			IStronglyTypedMetadata metadata)
		{
			Value = default;

			status = EPoolElementStatus.UNINITIALIZED;

			this.metadata = metadata;
		}

		#region IAsyncPoolElementFacadeWithMetadata

		#region IAsyncPoolElementFacade

		public T Value { get; set; }

		public EPoolElementStatus Status
		{
			get => status;
			set => status = value;
		}

		public IAsyncManagedPool<T> Pool
		{
			get => pool;
			set => pool = value;
		}

		public async Task Push(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (status == EPoolElementStatus.PUSHED)
				return;

			if (pool == null)
				return;

			await pool.Push(
				this,
				asyncContext);
		}

		#endregion

		public IStronglyTypedMetadata Metadata
		{
			get => metadata;
		}

		#endregion

		#region ICleanUppable

		public virtual void Cleanup()
		{
			Push(null);

			if (Value is ICleanuppable)
				(Value as ICleanuppable).Cleanup();

			if (metadata is ICleanuppable)
				(metadata as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public virtual void Dispose()
		{
			Push(null);

			if (Value is IDisposable)
				(Value as IDisposable).Dispose();

			if (metadata is IDisposable)
				(metadata as IDisposable).Dispose();
		}

		#endregion
	}
}