using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Factories
{
	public class ConfigurableStackPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConfigurableStackPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public ConfigurableStackPool<T> BuildConfigurableStackPool<T>(
			IAllocationCommand<T> initialAllocationCommand,
			IAllocationCommand<T> additionalAllocationCommand)
		{
			var stack = new Stack<T>();

			PerformInitialAllocation<T>(
				stack,
				initialAllocationCommand);

			return new ConfigurableStackPool<T>(
				stack,
				additionalAllocationCommand,
				this);
		}

		private void PerformInitialAllocation<T>(
			Stack<T> stack,
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
						$"[ConfigurableStackPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = initialAllocationCommand.AllocationDelegate();

				initialAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				stack.Push(
					newElement);
			}
		}

		#endregion

		#region Resize

		public int ResizeConfigurableStackPool<T>(
			Stack<T> stack,
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
						$"[ConfigurableStackPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(newElement);

				stack.Push(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}