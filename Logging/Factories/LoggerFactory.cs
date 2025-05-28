using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging.Builders;

namespace HereticalSolutions.Logging.Factories
{
    public class LoggerFactory
    {
        private readonly RepositoryFactory repositoryFactory;

        public LoggerFactory(
            RepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public LoggerBuilder BuildLoggerBuilder()
        {
            return new LoggerBuilder(
                repositoryFactory,
                this);
        }

        public LoggerResolver BuildLoggerResolver(
            ILogger rootLogger,
            IRepository<Type, bool> explicitLogSourceRules,
            bool allowedByDefault)
        {
            return new LoggerResolver(
                rootLogger,
                explicitLogSourceRules,
                allowedByDefault);
        }

        #region Wrappers

        public ProxyWrapper BuildProxyWrapper()
        {
            return new ProxyWrapper();
        }

        public CompositeLoggerWrapper BuildCompositeLoggerWrapper()
        {
            return new CompositeLoggerWrapper(
                new List<ILogger>());
        }

        public LoggerWrapperWithSemaphoreSlim
            BuildLoggerWrapperWithSemaphoreSlim()
        {
            return new LoggerWrapperWithSemaphoreSlim(
                new SemaphoreSlim(1, 1));
        }

        public LoggerWrapperWithSourceTypePrefix
            BuildLoggerWrapperWithSourceTypePrefix()
        {
            return new LoggerWrapperWithSourceTypePrefix();
        }

        public LoggerWrapperWithLogTypePrefix
            BuildLoggerWrapperWithLogTypePrefix()
        {
            return new LoggerWrapperWithLogTypePrefix();
        }
        
        public LoggerWrapperWithThreadIndexPrefix 
            BuildLoggerWrapperWithThreadIndexPrefix()
        {
            return new LoggerWrapperWithThreadIndexPrefix();
        }
        
        public LoggerWrapperWithTimestampPrefix 
            BuildLoggerWrapperWithTimestampPrefix(
                bool utc)
        {
            return new LoggerWrapperWithTimestampPrefix(
                utc);
        }
        
        public LoggerWrapperWithRecursionPreventionPrefix 
            BuildLoggerWrapperWithRecursionPreventionPrefix()
        {
            return new LoggerWrapperWithRecursionPreventionPrefix();
        }
        
        public LoggerWrapperWithRecursionPreventionGate 
            BuildLoggerWrapperWithRecursionPreventionGate()
        {
            return new LoggerWrapperWithRecursionPreventionGate();
        }

        public LoggerWrapperWithToggling BuildLoggerWrapperWithToggling(
            bool active = true,
            bool logsActive = true,
            bool warningsActive = true,
            bool errorsActive = true)
        {
            return new LoggerWrapperWithToggling(
                active,
                logsActive,
                warningsActive,
                errorsActive);
        }

        #endregion

        #region Sinks

        public ConsoleSink BuildConsoleSink()
        {
            return new ConsoleSink();
        }

        public FileSink BuildFileSink(
            PersistenceFactory persistenceFactory,
            IPathSettings pathSettings)
        {
            var serializerBuilder = persistenceFactory.BuildSerializerBuilder();

            var serializer = serializerBuilder
                .NewSerializer()
                .From<IPathSettings>(
                    pathSettings)
                .ToObject()
                .AsTextStream()
                .WithAppend()
                .BuildSerializer();
                
            var result = new FileSink(
                serializer);

            return result;
        }

        #endregion
    }
}