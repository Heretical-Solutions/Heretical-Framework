using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Async.Factories
{
	public class AsyncConfigurableLinkedListPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AsyncConfigurableLinkedListPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncConfigurableLinkedListPool<T>>
			BuildAsyncConfigurableLinkedListPool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			var linkedList = new LinkedList<T>();

			await PerformInitialAllocation<T>(
				linkedList,
				initialAllocationCommand,
				asyncContext);

			return new AsyncConfigurableLinkedListPool<T>(
				linkedList,
				additionalAllocationCommand,
				this);
		}

		private async Task PerformInitialAllocation<T>(
			LinkedList<T> linkedList,
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
						$"[AsyncConfigurableLinkedListPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await initialAllocationCommand.AllocationDelegate();

				initialAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				linkedList.AddFirst(
					newElement);
			}
		}

		#endregion

		#region Resize

		public async Task<int> ResizeAsyncConfigurableLinkedListPool<T>(
			LinkedList<T> linkedList,
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
						$"[AsyncConfigurableLinkedListPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				linkedList.AddFirst(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}