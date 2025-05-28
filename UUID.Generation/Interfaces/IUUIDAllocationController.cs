namespace HereticalSolutions.UUID.Generation
{
	public interface IUUIDAllocationController<TID>
	{
		bool ValidateUUID(
			TID id);

		bool IsAllocated(
			TID id);

		bool AllocateUUID(
			out TID id);

		bool FreeUUID(
			TID id);
	}
}