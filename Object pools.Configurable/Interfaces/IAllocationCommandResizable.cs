using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools
{
	public interface IAllocationCommandResizable<T>
	{
		void Resize(
			IAllocationCommand<T> allocationCommand);
	}
}