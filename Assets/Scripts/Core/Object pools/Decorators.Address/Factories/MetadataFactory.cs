using HereticalSolutions.Pools.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class AddressDecoratorsPoolsFactory
    {
        #region Metadata

        public static AddressMetadata BuildAddressMetadata()
        {
            return new AddressMetadata();
        }

        public static MetadataAllocationDescriptor BuildAddressMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsAddress),
                ConcreteType = typeof(AddressMetadata)
            };
        }

        #endregion
    }
}