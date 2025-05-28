using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
	public class PackedArrayManagedPoolFactory
	{
		private readonly PoolElementFacadeFactory poolElementFacadeFactory;

		private readonly ManagedObjectPoolAllocationCommandFactory
			managedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public PackedArrayManagedPoolFactory(
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

		public PackedArrayManagedPool<T> BuildPackedArrayManagedPool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>>
				facadeAllocationCallback = null,
				
			bool validateValues = true)
		{
			ILogger logger =
				loggerResolver?.GetLogger<PackedArrayManagedPool<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => poolElementFacadeFactory.
					BuildPoolElementFacadeWithArrayIndex<T>(
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

			int initialAmount = initialFacadeAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var exceptionLogger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					exceptionLogger.TryFormatException(
						$"[PackedArrayManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialFacadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			IPoolElementFacade<T>[] contents =
				new IPoolElementFacade<T>[initialAmount];

			PerformInitialAllocation<T>(
				initialAmount,
				contents,

				initialFacadeAllocationCommand,
				initialAllocationCommand);

			return new PackedArrayManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				contents,

				logger,

				validateValues);
		}

		private void PerformInitialAllocation<T>(
			int initialAmount,
			IPoolElementFacade<T>[] contents,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(
				//    newElement);

				var newElementValue = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(
					newElementValue);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = EPoolElementStatus.PUSHED;

				facadeAllocationCommand.AllocationCallback?.OnAllocated(
					newElementFacade);

				contents[i] = newElementFacade;
			}
		}

		#endregion

		#region Resize

		public IPoolElementFacade<T>[] ResizePackedArrayManagedPool<T>(
			IPoolElementFacade<T>[] packedArray,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand,

			bool newValuesAreInitialized)
		{
			int currentCapacity = packedArray.Length;

			int addedCapacity = facadeAllocationCommand
				.Descriptor
				.CountAdditionalAllocationAmount(
					currentCapacity);

			if (addedCapacity == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[PackedArrayMangedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			int newCapacity = currentCapacity + addedCapacity;

			IPoolElementFacade<T>[] newContents =
				new IPoolElementFacade<T>[newCapacity];

			FillNewArrayWithContents(
				packedArray,
				newContents,

				facadeAllocationCommand,
				valueAllocationCommand,
				
				newValuesAreInitialized);

			return newContents;
		}

		private void FillNewArrayWithContents<T>(
			IPoolElementFacade<T>[] oldContents,
			IPoolElementFacade<T>[] newContents,

			IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			IAllocationCommand<T> valueAllocationCommand,

			bool newValuesAreInitialized)
		{
			for (int i = 0; i < oldContents.Length; i++)
				newContents[i] = oldContents[i];

			for (int i = oldContents.Length; i < newContents.Length; i++)
			{
				var newElementFacade = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(
				//    newElement);

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


				newContents[i] = newElementFacade;
			}
		}

		#endregion
	}
}