using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Configurable.Async.Factories
{
	public class AsyncConfigurablePackedArrayPoolFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AsyncConfigurablePackedArrayPoolFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncConfigurablePackedArrayPool<T>>
			BuildAsyncConfigurablePackedArrayPool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			var logger = loggerResolver?
				.GetLogger<AsyncConfigurablePackedArrayPool<T>>();

			int initialAmount = initialAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var exceptionLogger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					exceptionLogger.TryFormatException(
						$"[AsyncConfigurablePackedArrayPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			T[] contents = new T[initialAmount];

			await PerformInitialAllocation<T>(
				initialAmount,
				contents,
				initialAllocationCommand,
				asyncContext);

			return new AsyncConfigurablePackedArrayPool<T>(
				contents,
				additionalAllocationCommand,
				this,
				logger);
		}

		private async Task PerformInitialAllocation<T>(
			int initialAmount,
			T[] contents,
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				contents[i] = newElement;
			}
		}

		#endregion

		#region Resize

		public async Task<T[]> ResizeAsyncConfigurablePackedArrayPool<T>(
			T[] packedArray,
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
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
						$"[AsyncConfigurablePackedArrayPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			int newCapacity = currentCapacity + addedCapacity;

			T[] newContents = new T[newCapacity];

			await FillNewArrayWithContents<T>(
				packedArray,
				newContents,
				allocationCommand,
				asyncContext);

			return newContents;
		}

		#endregion

		private async Task FillNewArrayWithContents<T>(
			T[] oldContents,
			T[] newContents,
			IAsyncAllocationCommand<T> allocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < oldContents.Length; i++)
				newContents[i] = oldContents[i];

			for (int i = oldContents.Length; i < newContents.Length; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElement,
						asyncContext);

				newContents[i] = newElement;
			}
		}
	}
}