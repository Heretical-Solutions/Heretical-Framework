using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.UUID.Mapping;
using HereticalSolutions.UUID.Generation;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Address.Factories
{
    public class AddressDecoratorPoolFactory
    {
        private readonly RepositoryFactory repositoryFactory;

        private readonly OneToOneMapFactory oneToOneMapFactory;

        private readonly ILoggerResolver loggerResolver;

        public AddressDecoratorPoolFactory(
            RepositoryFactory repositoryFactory,
            OneToOneMapFactory oneToOneMapFactory,
            ILoggerResolver loggerResolver)
        {
            this.repositoryFactory = repositoryFactory;

            this.oneToOneMapFactory = oneToOneMapFactory;

            this.loggerResolver = loggerResolver;
        }

        public AddressDecoratorManagedPool<TUUID, T>
            BuildAddressDecoratorManagedPool<TUUID, T>(
                IUUIDAllocationController<TUUID> uuidAllocationController)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AddressDecoratorManagedPool<TUUID, T>>();

            IRepository<TUUID, IManagedPool<T>> innerPoolRepository =
                repositoryFactory.BuildDictionaryRepository<TUUID, IManagedPool<T>>();

            //TODO: extract to factory
            IUUIDRegistry<TUUID> uuidRegistry = new UUIDRegistry<TUUID>(
                oneToOneMapFactory.BuildDictionaryOneToOneMap<string, TUUID>());

            return new AddressDecoratorManagedPool<TUUID, T>(
                uuidAllocationController,
                innerPoolRepository,
                uuidRegistry,
                logger);
        }
    }
}