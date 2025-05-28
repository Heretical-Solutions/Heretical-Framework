namespace HereticalSolutions.ObjectPools.Managed
{
    public interface IPoolElementFacade<T>
    {
        T Value { get; set; }

        EPoolElementStatus Status { get; set; }

        IManagedPool<T> Pool { get; set; }

        void Push();
    }
}