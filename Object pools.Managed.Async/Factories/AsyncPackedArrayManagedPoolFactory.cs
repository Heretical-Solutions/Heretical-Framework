using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncPackedArrayManagedPoolFactory
	{
		private readonly AsyncPoolElementFacadeFactory asyncPoolElementFacadeFactory;

		private readonly AsyncManagedObjectPoolAllocationCommandFactory
			asyncManagedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public AsyncPackedArrayManagedPoolFactory(
			AsyncPoolElementFacadeFactory asyncPoolElementFacadeFactory,
			AsyncManagedObjectPoolAllocationCommandFactory
				asyncManagedObjectPoolAllocationCommandFactory,
			ILoggerResolver loggerResolver)
		{
			this.asyncPoolElementFacadeFactory = asyncPoolElementFacadeFactory;

			this.asyncManagedObjectPoolAllocationCommandFactory =
				asyncManagedObjectPoolAllocationCommandFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Build

		public async Task<AsyncPackedArrayManagedPool<T>> 
			BuildAsyncPackedArrayManagedPool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext,

				MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
				IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
					facadeAllocationCallback = null,
				
				bool validateValues = true)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncPackedArrayManagedPool<T>>();

			//ALTERNATIVE
			//Func<Task<IAsyncPoolElementFacade<T>>> facadeAllocationDelegate =
			//	() => Task.FromResult<IAsyncPoolElementFacade<T>>(
			//		asyncPoolElementFacadeFactory.
			//			BuildAsyncPoolElementFacade<T>(
			//				metadataAllocationDescriptors));

			//ALTERNATIVE
			Func<Task<IAsyncPoolElementFacade<T>>> facadeAllocationDelegate =
				async () =>
				{
					var facade = asyncPoolElementFacadeFactory.
						BuildAsyncPoolElementFacade<T>(
							metadataAllocationDescriptors);

					return facade;
				};

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
				initialFacadeAllocationCommand =
					asyncManagedObjectPoolAllocationCommandFactory.
						BuildAsyncPoolElementFacadeAllocationCommand(
							initialAllocationCommand.Descriptor,
							facadeAllocationDelegate,
							facadeAllocationCallback);

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
				additionalFacadeAllocationCommand =
					asyncManagedObjectPoolAllocationCommandFactory.
						BuildAsyncPoolElementFacadeAllocationCommand(
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
						$"[AsyncPackedArrayManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialFacadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			IAsyncPoolElementFacade<T>[] contents =
				new IAsyncPoolElementFacade<T>[initialAmount];

			await PerformInitialAllocation<T>(
				initialAmount,
				contents,

				initialFacadeAllocationCommand,
				initialAllocationCommand,
				
				asyncContext);

			return new AsyncPackedArrayManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				contents,

				logger,

				validateValues);
		}

		private async Task PerformInitialAllocation<T>(
			int initialAmount,
			IAsyncPoolElementFacade<T>[] contents,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> 
				facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(
				//    newElement);

				var newElementValue = await valueAllocationCommand
					.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementValue,
						asyncContext);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = EPoolElementStatus.PUSHED;

				await facadeAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementFacade,
						asyncContext);

				contents[i] = newElementFacade;
			}
		}

		#endregion

		#region Resize

		public async Task<IAsyncPoolElementFacade<T>[]>
			ResizeAsyncPackedArrayManagedPool<T>(
				IAsyncPoolElementFacade<T>[] packedArray,
	
				IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> 
					facadeAllocationCommand,
				IAsyncAllocationCommand<T> valueAllocationCommand,
	
				bool newValuesAreInitialized,

				//Async tail
				AsyncExecutionContext asyncContext)
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
						$"[AsyncPackedArrayMangedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			int newCapacity = currentCapacity + addedCapacity;

			IAsyncPoolElementFacade<T>[] newContents =
				new IAsyncPoolElementFacade<T>[newCapacity];

			await FillNewArrayWithContents(
				packedArray,
				newContents,

				facadeAllocationCommand,
				valueAllocationCommand,

				newValuesAreInitialized,
				
				asyncContext);

			return newContents;
		}

		private async Task FillNewArrayWithContents<T>(
			IAsyncPoolElementFacade<T>[] oldContents,
			IAsyncPoolElementFacade<T>[] newContents,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> 
				facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			for (int i = 0; i < oldContents.Length; i++)
				newContents[i] = oldContents[i];

			for (int i = oldContents.Length; i < newContents.Length; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(
				//    newElement);

				var newElementValue = await valueAllocationCommand
					.AllocationDelegate();

				await valueAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementValue,
						asyncContext);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = (newValuesAreInitialized)
					? EPoolElementStatus.PUSHED
					: EPoolElementStatus.UNINITIALIZED;

				await facadeAllocationCommand
					.AllocationCallback?
					.OnAllocated(
						newElementFacade,
						asyncContext);


				newContents[i] = newElementFacade;
			}
		}

		#endregion
	}
}