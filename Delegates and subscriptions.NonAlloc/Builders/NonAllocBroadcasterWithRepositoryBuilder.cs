using System;

using HereticalSolutions.Builders;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Delegates.NonAlloc.Factories;

namespace HereticalSolutions.Delegates.NonAlloc.Builders
{
    public class NonAllocBroadcasterWithRepositoryBuilder
        : ABuilder<NonAllocBroadcasterWithRepositoryBuilderContext>
    {
        private readonly NonAllocBroadcasterFactory nonAllocBroadcasterFactory;

        private readonly RepositoryFactory repositoryFactory;

        public NonAllocBroadcasterWithRepositoryBuilder(
            NonAllocBroadcasterFactory nonAllocBroadcasterFactory,
            RepositoryFactory repositoryFactory)
        {
            this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;

            this.repositoryFactory = repositoryFactory;

            context = null;
        }

        public NonAllocBroadcasterWithRepositoryBuilder
            New()
        {
            context = new NonAllocBroadcasterWithRepositoryBuilderContext
            {
                NonAllocBroadcasterFactory = nonAllocBroadcasterFactory,

                RepositoryFactory = repositoryFactory,

                BroadcasterRepository = repositoryFactory.
                    BuildDictionaryRepository<Type, object>()
            };

            return this;
        }

        public NonAllocBroadcasterWithRepositoryBuilder
            Add<TBroadcaster>()
        {
            context.BroadcasterRepository.Add(
                typeof(TBroadcaster),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<TBroadcaster>());

            return this;
        }

        public NonAllocBroadcasterWithRepository
            BuildNonAllocBroadcasterWithRepository()
        {
            var result = nonAllocBroadcasterFactory
                .BuildNonAllocBroadcasterWithRepository(
                    context.BroadcasterRepository);

            Cleanup();

            return result;
        }
    }
}