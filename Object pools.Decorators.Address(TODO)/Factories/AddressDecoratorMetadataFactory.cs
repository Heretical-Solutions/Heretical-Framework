using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Decorators.Address.Factories
{
    public class AddressDecoratorMetadataFactory
    {
        public AddressMetadata<TUUID> BuildAddressMetadata<TUUID>()
        {
            return new AddressMetadata<TUUID>();
        }

        public MetadataAllocationDescriptor BuildAddressMetadataDescriptor<TUUID>()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsAddress<TUUID>),
                
                ConcreteType = typeof(AddressMetadata<TUUID>)
            };
        }
    }
}