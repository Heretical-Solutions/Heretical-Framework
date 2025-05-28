/*
using System;
using System.Threading;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AConcurrentReadWriteResourceStorageHandle<TResource>
		: AConcurrentReadOnlyResourceStorageHandle<TResource>,
		  IResourceStorageHandle
	{
		public AConcurrentReadWriteResourceStorageHandle(
			SemaphoreSlim semaphore,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
			: base(
				semaphore,
				runtimeResourceManager,
				logger)
		{
		}

		#region IResourceStorageHandle

		public bool SetRawResource(object rawResource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					return false;
				}

				this.resource = (TResource)rawResource;

				return true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool SetResource<TValue>(TValue resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!allocated)
				{
					return false;
				}

				switch (resource)
				{
					case TResource targetTypeResource:

						this.resource = targetTypeResource;

						break;

					default:

						throw new Exception(
							logger.TryFormatException(
								GetType(),
								$"CANNOT SET RESOURCE OF TYPE {typeof(TValue).Name} TO RESOURCE OF TYPE {nameof(TResource)}"));
				}

				return true;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		#endregion
	}
}
*/