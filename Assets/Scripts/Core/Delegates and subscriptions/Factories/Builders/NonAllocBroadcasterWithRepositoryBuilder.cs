using System;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    public class NonAllocBroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        public NonAllocBroadcasterWithRepositoryBuilder()
        {
            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public NonAllocBroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(typeof(TBroadcaster), DelegatesFactory.BuildNonAllocBroadcasterGeneric<TBroadcaster>());

            return this;
        }

        public NonAllocBroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildNonAllocBroadcasterWithRepository(broadcastersRepository);
        }
    }
}