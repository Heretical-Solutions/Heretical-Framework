using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
	public class QueueManagedPoolFactory
	{
		private readonly PoolElementFacadeFactory poolElementFacadeFactory;

		private readonly ManagedObjectPoolAllocationCommandFactory
			managedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public QueueManagedPoolFactory(
			PoolElementFacadeFactory poolElementFacadeFactory,
			ManagedObjectPoolAllocationCommandFactory
				managedObjectPoolAllocationCommandFactory,
			ILoggerResolver loggerResolver)
		{
			this.poolElementFacadeFactory = poolElementFacadeFactory;

			this.managedObjectPoolAllocationCommandFactory =
				managedObjectPoolAllocationCommandFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Build

		public QueueManagedPool<T> BuildQueueManagedPool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>>
				facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<QueueManagedPool<T>>();

			var queue = new Queue<IPoolElementFacade<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => poolElementFacadeFactory.BuildPoolElementFacade<T>(
					metadataAllocationDescriptors);

			IAllocationCommand<IPoolElementFacade<T>>
				initialFacadeAllocationCommand =
					managedObjectPoolAllocationCommandFactory.
						BuildPoolElementFacadeAllocationCommand(
							initialAllocationCommand.Descriptor,
							facadeAllocationDelegate,
							facadeAllocationCallback);

			IAllocationCommand<IPoolElementFacade<T>> 
				additionalFacadeAllocationCommand =
					managedObjectPoolAllocationCommandFactory.
						BuildPoolElementFacadeAllocationCommand(
							additionalAllocationCommand.Descriptor,
							facadeAllocationDelegate,
							facadeAllocationCallback);

			PerformInitialAllocation<T>(
				queue,

				initialFacadeAllocationCommand,
				initialAllocationCommand);

			return new QueueManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,
				
				queue,
				
				logger);
		}

		private void PerformInitialAllocation<T>(
			Queue<IPoolElementFacade<T>> queue,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand)
		{
			int initialAmount = facadeAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[QueueManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);

				var newElementValue = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(
					newElementValue);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = EPoolElementStatus.PUSHED;

				facadeAllocationCommand.AllocationCallback?.OnAllocated(
					newElementFacade);

				queue.Enqueue(
					newElementFacade);
			}
		}

		#endregion

		#region Resize

		public int ResizeQueueManagedPool<T>(
			Queue<IPoolElementFacade<T>> queue,
			int currentCapacity,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand,

			bool newValuesAreInitialized)
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
						$"[QueueManagedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElementFacade = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				var newElementValue = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(
					newElementValue);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = (newValuesAreInitialized)
					? EPoolElementStatus.PUSHED
					: EPoolElementStatus.UNINITIALIZED;

				facadeAllocationCommand.AllocationCallback?.OnAllocated(
					newElementFacade);

				queue.Enqueue(
					newElementFacade);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}