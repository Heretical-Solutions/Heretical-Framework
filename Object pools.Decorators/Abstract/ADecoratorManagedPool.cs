using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators
{
    public abstract class ADecoratorManagedPool<T>
        : IManagedPool<T>,
          ICleanuppable,
          IDisposable
    {
        protected readonly IManagedPool<T> innerPool;

        public IManagedPool<T> InnerPool => innerPool;

        public ADecoratorManagedPool(
            IManagedPool<T> innerPool)
        {
            this.innerPool = innerPool;
        }

        #region IManagedPool

        public virtual IPoolElementFacade<T> Pop()
        {
            OnBeforePop(null);

            IPoolElementFacade<T> result = innerPool.Pop();

            OnAfterPop(
                result,
                null);

            return result;
        }

        public virtual IPoolElementFacade<T> Pop(
            IPoolPopArgument[] args)
        {
            OnBeforePop(args);

            IPoolElementFacade<T> result = innerPool.Pop(args);

            OnAfterPop(result, args);

            return result;
        }
        
        public virtual void Push(
            IPoolElementFacade<T> instance)
        {
            OnBeforePush(instance);

            innerPool.Push(
                instance);

            OnAfterPush(instance);
        }
        
        #endregion

        protected virtual void OnBeforePop(
            IPoolPopArgument[] args)
        {
        }

        protected virtual void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
        }

        protected virtual void OnBeforePush(
            IPoolElementFacade<T> instance)
        {
        }

        protected virtual void OnAfterPush(
            IPoolElementFacade<T> instance)
        {
        }

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPool is ICleanuppable)
                (innerPool as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPool is IDisposable)
                (innerPool as IDisposable).Dispose();
        }

        #endregion
    }
}