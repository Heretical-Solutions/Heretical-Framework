using HereticalSolutions.ObjectPools.Managed;

namespace HereticalSolutions.ObjectPools.Decorators.Variants
{
    public class VariantContainer<T>
    {
        public float Chance;

        public IManagedPool<T> Pool;
    }
}