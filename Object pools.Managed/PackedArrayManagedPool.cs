using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Collections;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed
{
    public class PackedArrayManagedPool<T>
        : AManagedPool<T>
    {
		protected readonly PackedArrayManagedPoolFactory
			packedArrayManagedPoolFactory;

		protected readonly bool validateValues;
		
		protected readonly ILogger logger;
        

        protected IPoolElementFacade<T>[] packedArray;
        
        protected int allocatedCount;

        public PackedArrayManagedPool(
			PackedArrayManagedPoolFactory
				packedArrayManagedPoolFactory,

            IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            IAllocationCommand<T> valueAllocationCommand,

			IPoolElementFacade<T>[] packedArray,

			ILogger logger,

			bool validateValues = true)
			: base(
				facadeAllocationCommand,
				valueAllocationCommand)
        {
			this.packedArrayManagedPoolFactory = packedArrayManagedPoolFactory;


			this.packedArray = packedArray;

            this.validateValues = validateValues;
            
            this.logger = logger;

            
            allocatedCount = 0;
        }

        public override IPoolElementFacade<T> Pop()
        {
	        //Retrieve a pooled element

	        IPoolElementFacade<T> result = default;

	        //Resize the pool if necessary

	        if (allocatedCount >= packedArray.Length)
	        {
		        Resize();
	        }

	        int index = allocatedCount;
	        
	        result = packedArray[index];
            
	        allocatedCount++;
	        
	        //Update metadata

            IPoolElementFacadeWithMetadata<T> resultWithMetadata =
				result as IPoolElementFacadeWithMetadata<T>;

            if (resultWithMetadata == null)
            {
	            throw new Exception(
		            logger.TryFormatException(
						GetType(),
			            "PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
            }

            //Update index
            
            var indexedFacade = resultWithMetadata as IIndexed;

            if (indexedFacade == null)
            {
	            throw new Exception(
		            logger.TryFormatException(
						GetType(),
			            "PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
            }
            
            indexedFacade.Index = index;

            //Validate values

            if (validateValues
				&& resultWithMetadata.Status == EPoolElementStatus.UNINITIALIZED)
            {
	            var newElement = valueAllocationCommand.AllocationDelegate(); 
                
	            valueAllocationCommand.AllocationCallback?.OnAllocated(newElement);
                
	            result.Value = newElement;
            }
            
            //Validate pool

            if (result.Pool == null)
            {
	            result.Pool = this;
            }

            //Update facade

            result.Status = EPoolElementStatus.POPPED;
            
            return result;
        }
        
        public override void Push(IPoolElementFacade<T> instance)
        {
	        int lastAllocatedItemIndex = allocatedCount - 1;

	        if (lastAllocatedItemIndex < 0)
	        {
		        logger?.LogError(
					GetType(),
			        $"ATTEMPT TO PUSH AN ITEM WHEN NO ITEMS ARE ALLOCATED");
                
		        return;
	        }

	        // Validate values

	        if (validateValues
				&& instance.Status != EPoolElementStatus.POPPED)
	        {
		        return;
	        } 
	        
	        IPoolElementFacadeWithMetadata<T> resultWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

	        if (resultWithMetadata == null)
	        {
		        throw new Exception(
			        logger.TryFormatException(
						GetType(),
				        "PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
	        }

	        //Get index
	        
	        var indexedFacade = resultWithMetadata as IIndexed;

	        if (indexedFacade == null)
	        {
		        throw new Exception(
			        logger.TryFormatException(
						GetType(),
				        "PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
	        }

	        int instanceIndex = indexedFacade.Index;
	        

	        if (instanceIndex == -1)
	        {
		        logger?.LogError(
					GetType(),
			        $"ATTEMPT TO PUSH AN ITEM TO PACKED ARRAY IT DOES NOT BELONG TO");
                
		        return;
	        }

	        if (instanceIndex > lastAllocatedItemIndex)
	        {
		        logger?.LogError(
					GetType(),
			        $"ATTEMPT TO PUSH AN ALREADY PUSHED ITEM: {instanceIndex}");
                
		        return;
	        }

	        if (instanceIndex != lastAllocatedItemIndex)
	        {
		        //Update last allocated element's index

		        var lastAllocatedItem = packedArray[lastAllocatedItemIndex];
		        
		        IPoolElementFacadeWithMetadata<T> lastAllocatedItemWithMetadata =
			        lastAllocatedItem as IPoolElementFacadeWithMetadata<T>;

		        if (lastAllocatedItemWithMetadata == null)
		        {
			        throw new Exception(
				        logger.TryFormatException(
							GetType(),
					        "PACKED ARRAY MANAGED POOL ELEMENT HAS NO METADATA"));
		        }

		        var lastAllocatedItemAsIndexable = lastAllocatedItemWithMetadata as IIndexed;

		        if (lastAllocatedItemAsIndexable == null)
		        {
			        throw new Exception(
				        logger.TryFormatException(
							GetType(),
					        "PACKED ARRAY MANAGED POOL ELEMENT HAS NO INDEXED FACADE"));
		        }

		        lastAllocatedItemAsIndexable.Index = instanceIndex;        
	            
	            //Swap pushed element and last allocated element

		        var swap = packedArray[instanceIndex];

		        packedArray[instanceIndex] = packedArray[lastAllocatedItemIndex];

		        packedArray[lastAllocatedItemIndex] = swap;
	        }

	        //Update index

	        indexedFacade.Index = -1;
	        
	        //Update facade

	        instance.Status = EPoolElementStatus.PUSHED;

	        allocatedCount--;
        }

		public override IPoolElementFacade<T> PopFacade()
		{
			throw new InvalidOperationException();
		}

		public override void PushFacade(
			IPoolElementFacade<T> instance)
		{
			throw new InvalidOperationException();
		}
		
        public override void Resize()
        {
	        packedArray = packedArrayManagedPoolFactory.
				ResizePackedArrayManagedPool<T>(
					packedArray,
					
					facadeAllocationCommand,
					valueAllocationCommand,

					true);
        }

		public override void Resize(
			IAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized)
		{
			packedArray = packedArrayManagedPoolFactory.
				ResizePackedArrayManagedPool<T>(
					packedArray,

					facadeAllocationCommand,
					allocationCommand,

					newValuesAreInitialized);
		}

		#region IDynamicArray

		public int Capacity { get => packedArray.Length; }
        
        public int Count { get => allocatedCount; }
		
        public IPoolElementFacade<T> ElementAt(int index)
        {
	        return packedArray[index];
        }
        
        public IPoolElementFacade<T> this[int index]
        {
	        get
	        {
		        if (index >= allocatedCount || index < 0)
			        throw new Exception(
				        logger.TryFormatException(
							GetType(),
					        $"INVALID INDEX: {index} COUNT: {allocatedCount} CAPACITY: {Capacity}"));

		        return packedArray[index];
	        }
        }
		
        public IPoolElementFacade<T> Get(int index)
        {
	        if (index >= allocatedCount || index < 0)
		        throw new Exception(
			        logger.TryFormatException(
						GetType(),
				        $"INVALID INDEX: {index} COUNT: {allocatedCount} CAPACITY: {Capacity}"));

	        return packedArray[index];
        }

        #endregion
        
        #region ICleanUppable

        public override void Cleanup()
        {
            foreach (var item in packedArray)
                if (item is ICleanuppable)
                    (item as ICleanuppable).Cleanup();

            allocatedCount = 0;
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            foreach (var item in packedArray)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            for (int i = 0; i < packedArray.Length; i++)
            {
                packedArray[i] = null;
            }

            allocatedCount = 0;
        }

        #endregion
    }
}