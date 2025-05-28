using HereticalSolutions.Collections;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
    public class ManagedObjectPoolMetadataFactory
    {
        public IndexedMetadata BuildIndexedMetadata()
        {
            return new IndexedMetadata();
        }

        public MetadataAllocationDescriptor BuildIndexedMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IIndexed),
                ConcreteType = typeof(IndexedMetadata)
            };
        }
    }
}