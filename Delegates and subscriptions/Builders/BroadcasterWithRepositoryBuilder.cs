using System;

using HereticalSolutions.Builders;

using HereticalSolutions.Delegates.Factories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Builders
{
    public class BroadcasterWithRepositoryBuilder
        : ABuilder<BroadcasterWithRepositoryBuilderContext>
    {
        private readonly RepositoryFactory repositoryFactory;

        private readonly BroadcasterFactory broadcasterFactory;

        public BroadcasterWithRepositoryBuilder(
            RepositoryFactory repositoryFactory,
            BroadcasterFactory broadcasterFactory)
        {
            this.repositoryFactory = repositoryFactory;

            this.broadcasterFactory = broadcasterFactory;

            context = null;
        }

        public BroadcasterWithRepositoryBuilder
            New()
        {
            context = new BroadcasterWithRepositoryBuilderContext
            {
                RepositoryFactory = repositoryFactory,

                BroadcasterFactory = broadcasterFactory,

                BroadcasterRepository = repositoryFactory
                    .BuildDictionaryRepository<Type, object>()
            };

            return this;
        }

        public BroadcasterWithRepositoryBuilder
            Add<TValue>()
        {
            context.BroadcasterRepository.Add(
                typeof(TValue),
                broadcasterFactory.BuildBroadcasterGeneric<TValue>());

            return this;
        }

        public BroadcasterWithRepository
            BuildBroadcasterWithRepository()
        {
            var result = broadcasterFactory.BuildBroadcasterWithRepository(
                context.BroadcasterRepository);

            Cleanup();

            return result;
        }
    }
}