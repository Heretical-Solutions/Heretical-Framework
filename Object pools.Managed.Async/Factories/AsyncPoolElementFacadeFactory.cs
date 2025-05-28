using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Metadata.Factories;

namespace HereticalSolutions.ObjectPools.Managed.Async.Factories
{
	public class AsyncPoolElementFacadeFactory
	{
		private readonly MetadataFactory metadataFactory;

		public AsyncPoolElementFacadeFactory(
			MetadataFactory metadataFactory)
		{
			this.metadataFactory = metadataFactory;
		}

		public AsyncPoolElementFacade<T> BuildAsyncPoolElementFacade<T>(
			MetadataAllocationDescriptor[] metadataDescriptors = null)
		{
			var metadata = metadataFactory.BuildStronglyTypedMetadata(
				metadataDescriptors);

			return new AsyncPoolElementFacade<T>(
				metadata);
		}

		public AsyncPoolElementFacadeWithArrayIndex<T> 
			BuildAsyncPoolElementFacadeWithArrayIndex<T>(
				MetadataAllocationDescriptor[] metadataDescriptors = null)
		{
			var metadata = metadataFactory.BuildStronglyTypedMetadata(
				metadataDescriptors);

			return new AsyncPoolElementFacadeWithArrayIndex<T>(
				metadata);
		}

		public AsyncPoolElementFacadeWithLinkedListLink<T> 
			BuildAsyncPoolElementFacadeWithLinkedList<T>(
				MetadataAllocationDescriptor[] metadataDescriptors = null)
		{
			var metadata = metadataFactory.BuildStronglyTypedMetadata(
				metadataDescriptors);

			return new AsyncPoolElementFacadeWithLinkedListLink<T>(
				metadata);
		}
	}
}