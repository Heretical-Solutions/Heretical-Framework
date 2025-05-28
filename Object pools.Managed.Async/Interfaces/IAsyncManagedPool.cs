using HereticalSolutions.ObjectPools.Async;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public interface IAsyncManagedPool<T>
		: IAsyncPool<IAsyncPoolElementFacade<T>>
	{
	}
}