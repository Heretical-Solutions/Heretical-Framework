using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Metadata.Factories;

namespace HereticalSolutions.ObjectPools.Managed.Factories
{
    public class PoolElementFacadeFactory
    {
        private readonly MetadataFactory metadataFactory;

        public PoolElementFacadeFactory(
            MetadataFactory metadataFactory)
        {
            this.metadataFactory = metadataFactory;
        }

        public PoolElementFacade<T> BuildPoolElementFacade<T>(
            MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = metadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacade<T>(
                metadata);
        }
        
        public PoolElementFacadeWithArrayIndex<T> 
            BuildPoolElementFacadeWithArrayIndex<T>(
                MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = metadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacadeWithArrayIndex<T>(
                metadata);
        }
        
        public PoolElementFacadeWithLinkedListLink<T> 
            BuildPoolElementFacadeWithLinkedList<T>(
                MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = metadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacadeWithLinkedListLink<T>(
                metadata);
        }
    }
}