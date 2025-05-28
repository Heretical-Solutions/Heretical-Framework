using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Async.Factories
{
	public class AsyncConfigurableQueuePoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AsyncConfigurableQueuePoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncConfigurableQueuePool<T>> 
			BuildAsyncConfigurableQueuePool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			var queue = new Queue<T>();

			await PerformInitialAllocation<T>(
				queue,
				initialAllocationCommand,
				asyncContext);

			return new AsyncConfigurableQueuePool<T>(
				queue,
				additionalAllocationCommand,
				this);
		}

		private async Task PerformInitialAllocation<T>(
			Queue<T> queue,
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
						$"[AsyncConfigurableQueuePoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await initialAllocationCommand.AllocationDelegate();

				await initialAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				queue.Enqueue(
					newElement);
			}
		}

		#endregion

		#region Resize

		public async Task<int> ResizeAsyncConfigurableQueuePool<T>(
			Queue<T> queue,
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
						$"[AsyncConfigurableQueuePoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				queue.Enqueue(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}