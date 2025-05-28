using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
    public class CleanupDecoratorManagedPool<T>
        : ADecoratorManagedPool<T>
    {
        public CleanupDecoratorManagedPool(
            IManagedPool<T> innerPool)
            : base(
                innerPool)
        {
        }
        
        protected override void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
            var valueAsCleanUppable = instance.Value as ICleanuppable;
            
            valueAsCleanUppable?.Cleanup();
        }
        
        protected override void OnBeforePush(
            IPoolElementFacade<T> instance)
        {
            var instanceAsCleanUppable = instance.Value as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
    }
}