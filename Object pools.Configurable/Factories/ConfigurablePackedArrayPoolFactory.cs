using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Factories
{
	public class ConfigurablePackedArrayPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConfigurablePackedArrayPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public ConfigurablePackedArrayPool<T> BuildConfigurablePackedArrayPool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand)
		{
			var logger = loggerResolver?
				.GetLogger<ConfigurablePackedArrayPool<T>>();

			int initialAmount = initialAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var exceptionLogger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					exceptionLogger.TryFormatException(
						$"[ConfigurablePackedArrayPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			T[] contents = new T[initialAmount];

			PerformInitialAllocation<T>(
				initialAmount,
				contents,
				initialAllocationCommand);

			return new ConfigurablePackedArrayPool<T>(
				contents,
				additionalAllocationCommand,
				this,
				logger);
		}

		private void PerformInitialAllocation<T>(
			int initialAmount,
			T[] contents,
			IAllocationCommand<T> allocationCommand)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(
					newElement);

				contents[i] = newElement;
			}
		}

		#endregion

		#region Resize

		public T[] ResizeConfigurablePackedArrayPool<T>(
			T[] packedArray,
			IAllocationCommand<T> allocationCommand)
		{
			int currentCapacity = packedArray.Length;

			int addedCapacity = allocationCommand
				.Descriptor
				.CountAdditionalAllocationAmount(
					currentCapacity);

			if (addedCapacity == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[ConfigurablePackedArrayPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			int newCapacity = currentCapacity + addedCapacity;

			T[] newContents = new T[newCapacity];

			FillNewArrayWithContents<T>(
				packedArray,
				newContents,
				allocationCommand);

			return newContents;
		}

		#endregion

		private void FillNewArrayWithContents<T>(
			T[] oldContents,
			T[] newContents,
			IAllocationCommand<T> allocationCommand)
		{
			for (int i = 0; i < oldContents.Length; i++)
				newContents[i] = oldContents[i];

			for (int i = oldContents.Length; i < newContents.Length; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(
					newElement);

				newContents[i] = newElement;
			}
		}
	}
}