using System.Collections;

using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
    public class ListCleanupDecoratorManagedPool<T>
        : ADecoratorManagedPool<T>
    {
        public ListCleanupDecoratorManagedPool(
            IManagedPool<T> innerPool)
            : base(
                innerPool)
        {
        }
        
        protected override void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
            var instanceAsList = instance.Value as IList;
            
            instanceAsList?.Clear();
        }
        
        protected override void OnBeforePush(
            IPoolElementFacade<T> instance)
        {
            var instanceAsList = instance.Value as IList;
            
            instanceAsList?.Clear();
        }
    }
}