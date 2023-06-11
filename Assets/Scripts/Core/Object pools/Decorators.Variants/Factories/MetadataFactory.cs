using HereticalSolutions.Pools.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class VariantsDecoratorsPoolsFactory
    {
        #region Metadata

        public static VariantMetadata BuildVariantMetadata()
        {
            return new VariantMetadata();
        }

        public static MetadataAllocationDescriptor BuildVariantMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsVariant),
                ConcreteType = typeof(VariantMetadata)
            };
        }

        #endregion
    }
}