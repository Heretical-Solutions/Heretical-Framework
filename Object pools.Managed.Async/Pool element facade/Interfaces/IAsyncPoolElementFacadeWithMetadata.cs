using HereticalSolutions.Metadata;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public interface IAsyncPoolElementFacadeWithMetadata<T>
		: IAsyncPoolElementFacade<T>
	{
		IStronglyTypedMetadata Metadata { get; }
	}
}