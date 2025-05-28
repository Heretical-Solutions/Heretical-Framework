using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Decorators.Variants.Factories
{
    public class VariantDecoratorMetadataFactory
    {
        public VariantMetadata BuildVariantMetadata()
        {
            return new VariantMetadata();
        }

        public MetadataAllocationDescriptor BuildVariantMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsVariant),
                ConcreteType = typeof(VariantMetadata)
            };
        }
    }
}