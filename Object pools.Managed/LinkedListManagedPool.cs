using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Collections;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed
{
    public class LinkedListManagedPool<T> 
        : AManagedPool<T>
    {
        protected readonly LinkedListManagedPoolFactory
            linkedListManagedPoolFactory;

        protected readonly ILogger logger;
        
        protected ILinkedListLink<T> firstElement;
        
        public LinkedListManagedPool(
            LinkedListManagedPoolFactory linkedListManagedPoolFactory,

            IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            IAllocationCommand<T> valueAllocationCommand,

            ILinkedListLink<T> firstElement,
            int capacity,

            ILogger logger)
            : base(
                facadeAllocationCommand,
                valueAllocationCommand)
        {
            this.linkedListManagedPoolFactory = linkedListManagedPoolFactory;

            this.logger = logger;


            this.firstElement = firstElement;
            
            this.capacity = capacity;
        }

		public override IPoolElementFacade<T> PopFacade()
		{
            IPoolElementFacade<T> result = null;

            if (firstElement == null)
            {
                Resize();
            }

            var poppedElement = firstElement;

            firstElement = poppedElement.Next;

            poppedElement.Previous = null;

            poppedElement.Next = null;

            if (firstElement != null)
                firstElement.Previous = null;


            result = poppedElement as IPoolElementFacade<T>;

            if (result == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
            }

            return result;
        }

        public override void PushFacade(
            IPoolElementFacade<T> instance)
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

        public override void Resize()
        {
            linkedListManagedPoolFactory.ResizeLinkedListManagedPool(
                ref firstElement,
                ref capacity,
                
                facadeAllocationCommand,
                valueAllocationCommand,
                
                true);
        }

        public override void Resize(
            IAllocationCommand<T> allocationCommand,
            bool newValuesAreInitialized)
        {
            linkedListManagedPoolFactory.ResizeLinkedListManagedPool(
                ref firstElement,
                ref capacity,

                facadeAllocationCommand,
                allocationCommand,

                newValuesAreInitialized);
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