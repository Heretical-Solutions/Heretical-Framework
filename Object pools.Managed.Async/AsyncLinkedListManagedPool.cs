using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Collections;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Async.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncLinkedListManagedPool<T>
		: AAsyncManagedPool<T>
	{
		protected readonly AsyncLinkedListManagedPoolFactory
			asyncLinkedListManagedPoolFactory;

		protected readonly ILogger logger;

		protected ILinkedListLink<T> firstElement;

		public AsyncLinkedListManagedPool(
			AsyncLinkedListManagedPoolFactory
				asyncLinkedListManagedPoolFactory,

			IAsyncAllocationCommand<IAsyncPoolElementFacade<T>> facadeAllocationCommand,
			IAsyncAllocationCommand<T> valueAllocationCommand,

			ILinkedListLink<T> firstElement,
			int capacity,

			ILogger logger)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
		{
			this.asyncLinkedListManagedPoolFactory =
				asyncLinkedListManagedPoolFactory;

			this.logger = logger;


			this.firstElement = firstElement;

			this.capacity = capacity;
		}

		public override async Task<IAsyncPoolElementFacade<T>> PopFacade(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncPoolElementFacade<T> result = null;

			if (firstElement == null)
			{
				await Resize(asyncContext);
			}

			var poppedElement = firstElement;

			firstElement = poppedElement.Next;

			poppedElement.Previous = null;

			poppedElement.Next = null;

			if (firstElement != null)
				firstElement.Previous = null;


			result = poppedElement as IAsyncPoolElementFacade<T>;

			if (result == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
			}

			return result;
		}

		public override async Task PushFacade(
			IAsyncPoolElementFacade<T> instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var instanceAsLinkedListLink = instance as ILinkedListLink<T>;

			if (instanceAsLinkedListLink == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"LINKED LIST MANAGED POOL ELEMENT IS NOT A LINKED LIST LINK"));
			}

			var previousFirstElement = firstElement;

			firstElement = instanceAsLinkedListLink;

			if (previousFirstElement != null)
				previousFirstElement.Previous = firstElement;

			instanceAsLinkedListLink.Previous = null;

			instanceAsLinkedListLink.Next = previousFirstElement;
		}

		public override async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await asyncLinkedListManagedPoolFactory.
				ResizeAsyncLinkedListManagedPool(
					firstElement,
					capacity,
	
					facadeAllocationCommand,
					valueAllocationCommand,
	
					true,
					
					asyncContext);

			firstElement = result.Item1;

			capacity = result.Item2;
		}

		public override async Task Resize(
			IAsyncAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await asyncLinkedListManagedPoolFactory.
				ResizeAsyncLinkedListManagedPool(
					firstElement,
					capacity,

					facadeAllocationCommand,
					allocationCommand,

					newValuesAreInitialized,
					
					asyncContext);

			firstElement = result.Item1;

			capacity = result.Item2;
		}

		public override void Cleanup()
		{
			var currentLink = firstElement;

			while (currentLink != null)
			{
				var currentElement = currentLink as IPoolElementFacade<T>;

				if (currentElement == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
				}

				if (currentElement is ICleanuppable)
					(currentElement as ICleanuppable).Cleanup();

				currentLink = currentLink.Next;
			}
		}

		public override void Dispose()
		{
			var currentLink = firstElement;

			while (currentLink != null)
			{
				var currentElement = currentLink as IPoolElementFacade<T>;

				if (currentElement == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
				}

				if (currentElement is IDisposable)
					(currentElement as IDisposable).Dispose();

				currentLink = currentLink.Next;
			}
		}
	}
}