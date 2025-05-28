using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Managed
{
    public class StackManagedPool<T> 
        : AManagedPool<T>
    {
        protected readonly StackManagedPoolFactory stackManagedPoolFactory;

        protected readonly Stack<IPoolElementFacade<T>> pool;

        protected readonly ILogger logger;
        
        public StackManagedPool(
            StackManagedPoolFactory stackManagedPoolFactory,

            Stack<IPoolElementFacade<T>> pool,

            IAllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
            IAllocationCommand<T> valueAllocationCommand,

            ILogger logger)
            : base(
                facadeAllocationCommand,
                valueAllocationCommand)
        {
            this.stackManagedPoolFactory = stackManagedPoolFactory;

            this.pool = pool;

            this.logger = logger;

            capacity = this.pool.Count;
        }

        public override IPoolElementFacade<T> PopFacade()
        {
            IPoolElementFacade<T> result = null;

            if (pool.Count != 0)
            {
                result = pool.Pop();
            }
            else
            {
                Resize();

                result = pool.Pop();
            }

            return result;
        }

		public override void PushFacade(IPoolElementFacade<T> instance)
		{
            pool.Push(instance);
        }

        public override void Resize()
        {
            capacity = stackManagedPoolFactory.ResizeStackManagedPool(
                pool,
                capacity,

                facadeAllocationCommand,
                valueAllocationCommand,

                true);
        }

        public override void Resize(
            IAllocationCommand<T> allocationCommand,
            bool newValuesAreInitialized)
        {
            capacity = stackManagedPoolFactory.ResizeStackManagedPool(
                pool,
                capacity,

                facadeAllocationCommand,
                allocationCommand,

                newValuesAreInitialized);
        }

        public override void Cleanup()
        {
            foreach (var item in pool)
                if (item is ICleanuppable)
                    (item as ICleanuppable).Cleanup();

            pool.Clear();
        }
        public override void Dispose()
        {
            foreach (var item in pool)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            pool.Clear();
        }
    }
}