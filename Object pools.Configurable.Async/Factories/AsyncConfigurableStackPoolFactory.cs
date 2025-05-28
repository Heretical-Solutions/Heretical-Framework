using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Async.Factories
{
	public class AsyncConfigurableStackPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AsyncConfigurableStackPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncConfigurableStackPool<T>> 
			BuildAsyncConfigurableStackPool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			var stack = new Stack<T>();

			await PerformInitialAllocation<T>(
				stack,
				initialAllocationCommand,
				asyncContext);

			return new AsyncConfigurableStackPool<T>(
				stack,
				additionalAllocationCommand,
				this);
		}

		private async Task PerformInitialAllocation<T>(
			Stack<T> stack,
			IAsyncAllocationCommand<T> initialAllocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int initialAmount = initialAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[AsyncConfigurableStackPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await initialAllocationCommand.AllocationDelegate();

				await initialAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				stack.Push(
					newElement);
			}
		}

		#endregion

		#region Resize

		public async Task<int> ResizeAsyncConfigurableStackPool<T>(
			Stack<T> stack,
			int currentCapacity,
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
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
						$"[AsyncConfigurableStackPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				stack.Push(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}