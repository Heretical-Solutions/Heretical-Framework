using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Metadata.Factories
{
	public class MetadataFactory
	{
		private readonly RepositoryFactory repositoryFactory;

		public MetadataFactory(
			RepositoryFactory repositoryFactory)
		{
			this.repositoryFactory = repositoryFactory;
		}

		public StronglyTypedMetadata BuildStronglyTypedMetadata()
		{
			return new StronglyTypedMetadata(
				repositoryFactory.BuildDictionaryInstanceRepository());
		}

		public StronglyTypedMetadata BuildStronglyTypedMetadata(
			MetadataAllocationDescriptor[] metadataDescriptors)
		{
			var repository = repositoryFactory.BuildDictionaryInstanceRepository();

			if (metadataDescriptors != null)
				foreach (var descriptor in metadataDescriptors)
				{
					if (descriptor == null)
						continue;

					repository.Add(
						descriptor.BindingType,
						AllocationFactory.ActivatorAllocationDelegate(
							descriptor.ConcreteType));
				}

			return new StronglyTypedMetadata(
				repository);
		}

		public WeaklyTypedMetadata BuildWeaklyTypedMetadata()
		{
			return new WeaklyTypedMetadata(
				repositoryFactory.BuildDictionaryRepository<string, object>());
		}
	}
}