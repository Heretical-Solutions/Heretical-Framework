using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Factories
{
	public class ConfigurableLinkedListPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConfigurableLinkedListPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public ConfigurableLinkedListPool<T>
			BuildConfigurableLinkedListPool<T>(
				IAllocationCommand<T> initialAllocationCommand,
				IAllocationCommand<T> additionalAllocationCommand)
		{
			var linkedList = new LinkedList<T>();

			PerformInitialAllocation<T>(
				linkedList,
				initialAllocationCommand);

			return new ConfigurableLinkedListPool<T>(
				linkedList,
				additionalAllocationCommand,
				this);
		}

		private void PerformInitialAllocation<T>(
			LinkedList<T> linkedList,
			IAllocationCommand<T> initialAllocationCommand)
		{
			int initialAmount = initialAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();
			
			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[ConfigurableLinkedListPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = initialAllocationCommand.AllocationDelegate();

				initialAllocationCommand.AllocationCallback?.OnAllocated(
					newElement);

				linkedList.AddFirst(
					newElement);
			}
		}

		#endregion

		#region Resize

		public int ResizeConfigurableLinkedListPool<T>(
			LinkedList<T> linkedList,
			int currentCapacity,
			IAllocationCommand<T> allocationCommand)
		{
			int addedCapacity = allocationCommand
				.Descriptor
				.CountAdditionalAllocationAmount(
					currentCapacity);

			if (addedCapacity == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[ConfigurableLinkedListPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(
					newElement);

				linkedList.AddFirst(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}