using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools.Managed
{
	public interface IManagedPool<T>
		: IPool<IPoolElementFacade<T>>
	{
	}
}