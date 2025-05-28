using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Configurable.Factories;

namespace HereticalSolutions.ObjectPools.Configurable
{
    public class ConfigurableStackPool<T> 
        : IPool<T>,
          IResizable,
          IAllocationCommandResizable<T>,
          ICleanuppable,
          IDisposable
    {
        private readonly Stack<T> pool;

        private readonly IAllocationCommand<T> allocationCommand;
        
        private readonly ConfigurableStackPoolFactory factory;

        private int capacity;
        
        public ConfigurableStackPool(
            Stack<T> pool,
            IAllocationCommand<T> allocationCommand,
            ConfigurableStackPoolFactory factory)
        {
            this.pool = pool;

            this.allocationCommand = allocationCommand;

            this.factory = factory;

            capacity = this.pool.Count;
        }

        #region IPool

        public T Pop()
        {
            T result = default(T);

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
        
        public T Pop(
            IPoolPopArgument[] args)
        {
            return Pop();
        }

        public void Push(
            T instance)
        {
            pool.Push(instance);
        }

        #endregion

        #region IAllocationResizable

        public void Resize()
        {
            capacity = factory.ResizeConfigurableStackPool(
                pool,
                capacity,
                allocationCommand);
        }

        #endregion

        #region IAllocationCommandResizable

        public void Resize(
            IAllocationCommand<T> allocationCommand)
        {
            capacity = factory.ResizeConfigurableStackPool(
                pool,
                capacity,
                allocationCommand);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var item in pool)
                if (item is ICleanuppable)
                    (item as ICleanuppable).Cleanup();

            pool.Clear();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var item in pool)
                if (item is IDisposable)
                    (item as IDisposable).Dispose();

            pool.Clear();
        }

        #endregion
    }
}