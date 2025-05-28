using System.Collections;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
    public class ListCleanupDecoratorPool<T>
        : ADecoratorPool<T>
    {
        public ListCleanupDecoratorPool(
            IPool<T> innerPool)
            : base(innerPool)
        {
        }
        
        protected override void OnAfterPop(
            T instance,
            IPoolPopArgument[] args)
        {
            var instanceAsList = instance as IList;
            
            instanceAsList?.Clear();
        }
        
        protected override void OnBeforePush(
            T instance)
        {
            var instanceAsList = instance as IList;
            
            instanceAsList?.Clear();
        }
    }
}