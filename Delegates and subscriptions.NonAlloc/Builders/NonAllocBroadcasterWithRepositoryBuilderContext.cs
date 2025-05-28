using System;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Delegates.NonAlloc.Factories;

namespace HereticalSolutions.Delegates.NonAlloc.Builders
{
	public class NonAllocBroadcasterWithRepositoryBuilderContext
	{
		public NonAllocBroadcasterFactory NonAllocBroadcasterFactory;

		public RepositoryFactory RepositoryFactory;

		public IRepository<Type, object> BroadcasterRepository;
	}
}