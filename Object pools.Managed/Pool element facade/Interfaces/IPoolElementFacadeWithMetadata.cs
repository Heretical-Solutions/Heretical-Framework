using HereticalSolutions.Metadata;

namespace HereticalSolutions.ObjectPools.Managed
{
    public interface IPoolElementFacadeWithMetadata<T>
        : IPoolElementFacade<T>
    {
        IStronglyTypedMetadata Metadata { get; }
    }
}