using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Factories
{
	public class ConfigurableQueuePoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConfigurableQueuePoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public ConfigurableQueuePool<T> BuildConfigurableQueuePool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand)
		{
			var queue = new Queue<T>();

			PerformInitialAllocation<T>(
				queue,
				initialAllocationCommand);

			return new ConfigurableQueuePool<T>(
				queue,
				additionalAllocationCommand,
				this);
		}

		private void PerformInitialAllocation<T>(
			Queue<T> queue,
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
						$"[ConfigurableQueuePoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = initialAllocationCommand.AllocationDelegate();

				initialAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				queue.Enqueue(
					newElement);
			}
		}

		#endregion

		#region Resize

		public int ResizeConfigurableQueuePool<T>(
			Queue<T> queue,
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
						$"[ConfigurableQueuePoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(
					newElement);

				queue.Enqueue(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}