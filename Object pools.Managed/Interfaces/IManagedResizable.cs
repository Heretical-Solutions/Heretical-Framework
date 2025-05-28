using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools.Managed
{
	public interface IManagedResizable<T>
	{
		void Resize(
			IAllocationCommand<T> allocationCommand,
			bool newValuesAreInitialized);
	}
}