using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools.Managed
{
	public interface IManagedPoolInternal<T>
		
	{
		IAllocationCommand<IPoolElementFacade<T>> FacadeAllocationCommand { get; }

		IAllocationCommand<T> ValueAllocationCommand { get; }

		IPoolElementFacade<T> PopFacade();

		void PushFacade(
			IPoolElementFacade<T> facade);
	}
}