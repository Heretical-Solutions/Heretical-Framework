using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    public class BroadcasterWithRepositoryBuilder
    {
        private readonly IRepository<Type, object> broadcastersRepository;

        public BroadcasterWithRepositoryBuilder()
        {
            broadcastersRepository = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
        }

        public BroadcasterWithRepositoryBuilder Add<TBroadcaster>()
        {
            broadcastersRepository.Add(typeof(TBroadcaster), DelegatesFactory.BuildBroadcasterGeneric<TBroadcaster>());

            return this;
        }

        public BroadcasterWithRepository Build()
        {
            return DelegatesFactory.BuildBroadcasterWithRepository(broadcastersRepository);
        }
    }
}