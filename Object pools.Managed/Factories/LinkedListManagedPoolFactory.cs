using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Collections;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
	public class LinkedListManagedPoolFactory
	{
		private readonly PoolElementFacadeFactory poolElementFacadeFactory;

		private readonly ManagedObjectPoolAllocationCommandFactory
			managedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public LinkedListManagedPoolFactory(
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

		public LinkedListManagedPool<T> BuildLinkedListManagedPool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>>
				facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<LinkedListManagedPool<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => poolElementFacadeFactory.
					BuildPoolElementFacadeWithLinkedList<T>(
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

			var firstElement = PerformInitialAllocation<T>(
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				out var capacity);

			return new LinkedListManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				firstElement,
				capacity,

				logger);
		}

		private ILinkedListLink<T> PerformInitialAllocation<T>(
			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand,

			out int capacity)
		{
			int initialAmount = facadeAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[LinkedListManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			ILinkedListLink<T> firstElement = null;

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


				var newElementAsLink = newElementFacade as ILinkedListLink<T>;

				newElementAsLink.Previous = null;

				newElementAsLink.Next = firstElement;

				if (firstElement != null)
					firstElement.Previous = newElementAsLink;

				firstElement = newElementAsLink;
			}

			capacity = initialAmount;

			return firstElement;
		}

		#endregion

		#region Resize

		public void ResizeLinkedListManagedPool<T>(
			ref ILinkedListLink<T> firstElement,
			ref int currentCapacity,

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
						$"[LinkedListManagedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
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


				var newElementAsLink = newElementFacade as ILinkedListLink<T>;

				newElementAsLink.Previous = null;

				newElementAsLink.Next = firstElement;

				if (firstElement != null)
					firstElement.Previous = newElementAsLink;

				firstElement = newElementAsLink;
			}

			currentCapacity += addedCapacity;
		}

		#endregion
	}
}