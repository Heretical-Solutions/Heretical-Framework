using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.ObjectPools.Decorators.Cleanup
{
    public class CleanupDecoratorPool<T>
        : ADecoratorPool<T>
    {
        public CleanupDecoratorPool(
            IPool<T> innerPool)
            : base(innerPool)
        {
        }
        
        protected override void OnAfterPop(
        	T instance,
        	IPoolPopArgument[] args)
        {
            var instanceAsCleanUppable = instance as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
        
        protected override void OnBeforePush(
            T instance)
        {
            var instanceAsCleanUppable = instance as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
    }
}