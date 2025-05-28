using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Collections;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncLinkedListManagedPoolFactory
	{
		private readonly AsyncPoolElementFacadeFactory asyncPoolElementFacadeFactory;

		private readonly AsyncManagedObjectPoolAllocationCommandFactory
			asyncManagedObjectPoolAllocationCommandFactory;

		private readonly ILoggerResolver loggerResolver;

		public AsyncLinkedListManagedPoolFactory(
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

		public async Task<AsyncLinkedListManagedPool<T>> 
			BuildAsyncLinkedListManagedPool<T>(
				IAsyncAllocationCommand<T> initialAllocationCommand,
				IAsyncAllocationCommand<T> additionalAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext,

				MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
				IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>
					facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncLinkedListManagedPool<T>>();

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

			var allocationResult = await PerformInitialAllocation<T>(
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				asyncContext);

			var firstElement = allocationResult.Item1;

			var capacity = allocationResult.Item2;

			return new AsyncLinkedListManagedPool<T>(
				this,

				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				firstElement,
				capacity,

				logger);
		}

		private async Task<(ILinkedListLink<T>, int)>
			PerformInitialAllocation<T>(
				IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
					facadeAllocationCommand,
				IAsyncAllocationCommand<T> valueAllocationCommand,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			int initialAmount = facadeAllocationCommand
				.Descriptor
				.CountInitialAllocationAmount();

			if (initialAmount == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[AsyncLinkedListManagedPoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			ILinkedListLink<T> firstElement = null;

			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);

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


				var newElementAsLink = newElementFacade as ILinkedListLink<T>;

				newElementAsLink.Previous = null;

				newElementAsLink.Next = firstElement;

				if (firstElement != null)
					firstElement.Previous = newElementAsLink;

				firstElement = newElementAsLink;
			}

			int capacity = initialAmount;

			return (firstElement, capacity);
		}

		#endregion

		#region Resize

		public async Task<(ILinkedListLink<T>, int)> 
			ResizeAsyncLinkedListManagedPool<T>(
				ILinkedListLink<T> firstElement,
				int currentCapacity,
	
				IAsyncAllocationCommand<IAsyncPoolElementFacade<T>>
					facadeAllocationCommand,
				IAsyncAllocationCommand<T> valueAllocationCommand,
	
				bool newValuesAreInitialized,

				//Async tail
				AsyncExecutionContext asyncContext)
		{
			int addedCapacity = facadeAllocationCommand
				.Descriptor
				.CountAdditionalAllocationAmount(
					currentCapacity);

			if (addedCapacity == -1)
			{
				var logger = loggerResolver.GetDefaultLogger();

				throw new Exception(
					logger.TryFormatException(
						$"[AsyncLinkedListManagedPoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElementFacade = await facadeAllocationCommand
					.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);

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


				var newElementAsLink = newElementFacade as ILinkedListLink<T>;

				newElementAsLink.Previous = null;

				newElementAsLink.Next = firstElement;

				if (firstElement != null)
					firstElement.Previous = newElementAsLink;

				firstElement = newElementAsLink;
			}

			currentCapacity += addedCapacity;

			return (firstElement, currentCapacity);
		}

		#endregion
	}
}