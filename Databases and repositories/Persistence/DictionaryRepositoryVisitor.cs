using HereticalSolutions.TypeConversion;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
	public class DictionaryRepositoryVisitor<TKey, TValue>
		: ARepositoryVisitor<DictionaryRepository<TKey, TValue>, TKey, TValue>
	{
		private RepositoryFactory repositoryFactory;

		public DictionaryRepositoryVisitor(
			RepositoryFactory repositoryFactory,
			ITypeConverter<string> stringConverter,
			ILogger logger)
			: base(
				stringConverter,
				logger)
		{
			this.repositoryFactory = repositoryFactory;
		}

		protected override DictionaryRepository<TKey, TValue> AllocateRepository()
		{
			return repositoryFactory
				.BuildDictionaryRepository<TKey, TValue>();
		}
	}
}