using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.ObjectPools.Decorators
{
    public abstract class ADecoratorPool<T>
        : IPool<T>,
          ICleanuppable,
          IDisposable
    {
        protected readonly IPool<T> innerPool;

        public IPool<T> InnerPool => innerPool;

        public ADecoratorPool(
            IPool<T> innerPool)
        {
            this.innerPool = innerPool;
        }

        #region IPool

        public virtual T Pop()
        {
            OnBeforePop(null);

            T result = innerPool.Pop();

            OnAfterPop(result, null);

            return result;
        }

        public virtual T Pop(
            IPoolPopArgument[] args)
        {
            OnBeforePop(args);

            T result = innerPool.Pop(args);

            OnAfterPop(result, args);

            return result;
        }
        
        public virtual void Push(
            T instance)
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
            T instance,
            IPoolPopArgument[] args)
        {
        }

        protected virtual void OnBeforePush(
            T instance)
        {
        }

        protected virtual void OnAfterPush(
            T instance)
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