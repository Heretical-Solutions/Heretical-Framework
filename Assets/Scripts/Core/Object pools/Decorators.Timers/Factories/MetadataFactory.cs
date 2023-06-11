using HereticalSolutions.Pools.Allocations;

namespace HereticalSolutions.Pools.Factories
{
	public static partial class TimersDecoratorsPoolsFactory
	{
		#region Metadata

		public static RuntimeTimerMetadata BuildRuntimeTimerMetadata()
		{
			return new RuntimeTimerMetadata();
		}

		public static MetadataAllocationDescriptor BuildRuntimeTimerMetadataDescriptor()
		{
			return new MetadataAllocationDescriptor
			{
				BindingType = typeof(IContainsRuntimeTimer),
				ConcreteType = typeof(RuntimeTimerMetadata)
			};
		}

		#endregion
	}
}