using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncQueueManagedPoolFactory
	{
		private readonly AsyncPoolElementFacadeFactory asyncPoolElementFacadeFactory;

		private readonly AsyncManagedObjectPoolAllocationCommandFactory
			asyncManagedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public AsyncQueueManagedPoolFactory(
			AsyncPoolElementFacadeFactory asyncPoolElementFacadeFactory,
			AsyncManagedObjectPoolAllocationCommandFactory
				asyncManagedObjectPoolAllocationCommandFactory,
			ILoggerResolver loggerResolver)
		{
			this.asyncPoolElementFacadeFactory = asyncPoolElementFacadeFactory;

			this.asyncManagedObjectPoolAllocationCommandFactory =
				asyncManagedObjectPoolAllocationCommandFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncQueueManagedPool<T>> BuildAsyncQueueManagedPool<T>(
			IAsyncAllocationCommand<T> initialAllocationCommand,
			IAsyncAllocationCommand<T> additionalAllocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
				facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncQueueManagedPool<T>>();

			var queue = new Queue<IAsyncPoolElementFacade<T>>();

			//ALTERNATIVE
			//Func<Task<IAsyncPoolElementFacade<T>>> facadeAllocationDelegate =
			//	() => Task.FromResult<IAsyncPoolElementFacade<T>>(
			//		asyncPoolElementFacadeFactory.
			//			BuildAsyncPoolElementFacade<T>(
			//				metadataAllocationDescriptors));

			//ALTERNATIVE
			Func<Task<IAsyncPoolElementFacade<T>>> facadeAllocationDelegate =
				async () =>
				{
					var facade = asyncPoolElementFacadeFactory.
						BuildAsyncPoolElementFacade<T>(
							metadataAllocationDescriptors);

					return facade;
				};

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
				initialFacadeAllocationCommand =
					asyncManagedObjectPoolAllocationCommandFactory.
						BuildAsyncPoolElementFacadeAllocationCommand(
							initialAllocationCommand.Descriptor,
							facadeAllocationDelegate,
							facadeAllocationCallback);

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
				additionalFacadeAllocationCommand =
					asyncManagedObjectPoolAllocationCommandFactory.
						BuildAsyncPoolElementFacadeAllocationCommand(
							additionalAllocationCommand.Descriptor,
							facadeAllocationDelegate,
							facadeAllocationCallback);

			await PerformInitialAllocation<T>(
				queue,

				initialFacadeAllocationCommand,
				initialAllocationCommand,
				
				asyncContext);

			return new AsyncQueueManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				queue,

				logger);
		}

		private async Task PerformInitialAllocation<T>(
			Queue<IAsyncPoolElementFacade<T>> queue,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int initialAmount = facadeAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[AsyncQueueManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);

				var newElementValue = await valueAllocationCommand
					.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementValue,
						asyncContext);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = EPoolElementStatus.PUSHED;

				await facadeAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementFacade,
						asyncContext);

				queue.Enqueue(
					newElementFacade);
			}
		}

		#endregion

		#region Resize

		public async Task<int> ResizeAsyncQueueManagedPool<T>(
			Queue<IAsyncPoolElementFacade<T>> queue,
			int currentCapacity,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int addedCapacity = facadeAllocationCommand
				.Descriptor
				.CountAdditionalAllocationAmount(
					currentCapacity);

			if (addedCapacity == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[AsyncQueueManagedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				var newElementValue = await valueAllocationCommand
					.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementValue,
						asyncContext);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = (newValuesAreInitialized)
					? EPoolElementStatus.PUSHED
					: EPoolElementStatus.UNINITIALIZED;

				await facadeAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementFacade,
						asyncContext);

				queue.Enqueue(
					newElementFacade);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}