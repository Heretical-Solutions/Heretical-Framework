using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Builders
{
	public class BroadcasterWithRepositoryBuilderContext
	{
		public RepositoryFactory RepositoryFactory;

		public BroadcasterFactory BroadcasterFactory;

		public IRepository<Type, object> BroadcasterRepository;
	}
}