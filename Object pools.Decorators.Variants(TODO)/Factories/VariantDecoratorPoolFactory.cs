using HereticalSolutions.RandomGeneration;
using HereticalSolutions.RandomGeneration.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Variants.Factories
{
    public class VariantDecoratorPoolFactory
    {
        private readonly RepositoryFactory repositoryFactory;

        private readonly ILoggerResolver loggerResolver;

        public VariantDecoratorPoolFactory(
            RepositoryFactory repositoryFactory,
            ILoggerResolver loggerResolver)
        {
            this.repositoryFactory = repositoryFactory;

            this.loggerResolver = loggerResolver;
        }
        
        public VariantDecoratorManagedPool<T> BuildVariantDecoratorManagedPool<T>()
        {
            ILogger logger =
                loggerResolver?.GetLogger<VariantDecoratorManagedPool<T>>();

            return new VariantDecoratorManagedPool<T>(
                repositoryFactory.BuildDictionaryRepository<int, VariantContainer<T>>(),
                RandomFactory.BuildSystemRandomGenerator(),
                logger);
        }
        
        public VariantDecoratorManagedPool<T> BuildVariantDecoratorManagedPool<T>(
            IRepository<int, VariantContainer<T>> repository,
            IRandomGenerator generator)
        {
            ILogger logger =
                loggerResolver?.GetLogger<VariantDecoratorManagedPool<T>>();

            return new VariantDecoratorManagedPool<T>(
                repository,
                generator,
                logger);
        }
    }
}