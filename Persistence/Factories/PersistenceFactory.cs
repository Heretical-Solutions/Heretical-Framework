using System;
using System.Threading;
using System.Collections.Generic;
//using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Repositories;
//using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.TypeConversion;
using HereticalSolutions.TypeConversion.Factories;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Metadata.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
    public class PersistenceFactory
    {
        //private readonly RepositoryFactory repositoryFactory;

        private readonly MetadataFactory metadataFactory;

        private readonly TypeConversionFactory typeConversionFactory;

        private readonly ILoggerResolver loggerResolver;

        //private readonly Type[] formatSerializerTypes;

        //private readonly Type[] serializationArgumentTypes;

        //private readonly Type[] serializationMediumTypes;

        private readonly IRepository<Type, IEnumerable<IVisitor>> 
            defaultVisitorRepository;

        public PersistenceFactory(
            //RepositoryFactory repositoryFactory,
            MetadataFactory metadataFactory,
            TypeConversionFactory typeConversionFactory,
            ILoggerResolver loggerResolver)
        {
            //this.repositoryFactory = repositoryFactory;

            this.metadataFactory = metadataFactory;

            this.typeConversionFactory = typeConversionFactory;

            this.loggerResolver = loggerResolver;

            /*
            TypeHelpers.GetTypesWithAttributeInAllAssemblies<FormatSerializerAttribute>(
                out Type[] formatSerializerTypes);

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<SerializationArgumentAttribute>(
                out Type[] serializationArgumentTypes);

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<SerializationMediumAttribute>(
                out Type[] serializationMediumTypes);

            defaultVisitorRepository = CreateDefaultVisitorRepository();
            */
        }

        /*
        private IRepository<Type, IEnumerable<IVisitor>>
            CreateDefaultVisitorRepository()
        {
            IRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository =
                repositoryFactory.BuildDictionaryRepository<Type, IEnumerable<IVisitor>>();

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<VisitorAttribute>(
                out Type[] visitorTypes);

            foreach (Type visitorType in visitorTypes)
            {
                try
                {
                    object instance = null;

                    bool hasAnyConstructor = visitorType.GetConstructors().Length > 0;

                    bool hasDefaultConstructor = visitorType.GetConstructor(Type.EmptyTypes) != null;

                    if (!hasAnyConstructor)
                    {
                        instance = Activator.CreateInstance(
                            visitorType,
                            BindingFlags.CreateInstance
                                | BindingFlags.Public
                                | BindingFlags.Instance
                                | BindingFlags.OptionalParamBinding,
                            null,
                            new Object[] { Type.Missing },
                            null);
                    }
                    else if (hasDefaultConstructor)
                    {
                        instance = Activator.CreateInstance(visitorType);
                    }
                    else
                    {
                        //Meep Merp
                    }

                    if (instance == null)
                        continue;

                    if (instance is IVisitor visitor)
                    {
                        var attribute = visitorType.GetCustomAttribute<VisitorAttribute>(false);

                        var visitableType = attribute.TargetType;

                        if (concreteVisitorRepository.Has(visitableType))
                        {
                            var visitorList = concreteVisitorRepository
                                .Get(visitableType)
                                as List<IVisitor>;

                            visitorList.Add(visitor);
                        }
                        else
                        {
                            concreteVisitorRepository.Add(
                                visitableType,
                                new List<IVisitor> { visitor });
                        }
                    }
                }
                catch (MissingMethodException methodMissingException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"FAILED TO CREATE VISITOR INSTANCE FOR TYPE {visitorType.Name}",
                        e);
                }
            }

            return concreteVisitorRepository;
        }
        */

        #region Visitors

        public DispatchVisitor BuildDefaultDispatchVisitor()
        {
            return new DispatchVisitor(
                defaultVisitorRepository,
                Array.Empty<IVisitor>(),
                loggerResolver?.GetLogger<DispatchVisitor>());
        }

        public DispatchVisitor BuildDispatchVisitor(
            IRepository<Type, IEnumerable<IVisitor>> visitorRepository,
            IEnumerable<IVisitor> fallbackVisitors)
        {
            return new DispatchVisitor(
                visitorRepository,
                fallbackVisitors,
                loggerResolver?.GetLogger<DispatchVisitor>());
        }

        #endregion

        #region Arguments

        public AppendArgument BuildAppendArgument()
        {
            return new AppendArgument
            {
            };
        }

        public PathArgument BuildPathArgument(
            string path)
        {
            return new PathArgument
            {
                Path = path
            };
        }

        public BlockSerializationArgument BuildBlockSerializationArgument()
        {
            return new BlockSerializationArgument();
        }

        public FallbackToSyncArgument BuildFallbackToSyncArgument()
        {
            return new FallbackToSyncArgument();
        }

        public ReadAndWriteAccessArgument BuildReadAndWriteAccessArgument()
        {
            return new ReadAndWriteAccessArgument
            {
            };
        }

        public SourceByteArrayArgument BuildSourceByteArrayArgument(
            byte[] source)
        {
            return new SourceByteArrayArgument
            {
                Source = source
            };
        }

        public DataConversionArgument BuildDataConversionArgument()
        {
            return new DataConversionArgument();
        }

        public TSerializationArgument
            BuildSerializationArgument<TSerializationArgument>(
            object[] arguments = null)
        {
            throw new NotImplementedException(
                $"BuildSerializationArgument<{typeof(TSerializationArgument).Name}> is not implemented yet.");

            /*
            if (Array.IndexOf(
                serializationArgumentTypes,
                typeof(TSerializationArgument))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TSerializationArgument)} IS NOT A VALID SERIALIZATION ARGUMENT TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TSerializationArgument),
                arguments,
                loggerResolver);

            return (TSerializationArgument)Activator.CreateInstance(
                typeof(TSerializationArgument),
                 arguments);
            */
        }

        #endregion

        #region Format serializers

        public ObjectSerializer BuildObjectSerializer()
        {
            return new ObjectSerializer(
                loggerResolver?.GetLogger<ObjectSerializer>());
        }

        public BinaryFormatterSerializer BuildBinaryFormatterSerializer()
        {
            return new BinaryFormatterSerializer(
                new BinaryFormatter(),
                loggerResolver?.GetLogger<BinaryFormatterSerializer>());
        }

        public TFormatSerializer BuildFormatSerializer<TFormatSerializer>(
            object[] arguments = null)
            where TFormatSerializer : IFormatSerializer
        {
            throw new NotImplementedException(
                $"BuildFormatSerializer<{typeof(TFormatSerializer).Name}> is not implemented yet.");

            /*
            if (Array.IndexOf(
                formatSerializerTypes,
                typeof(TFormatSerializer))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TFormatSerializer)} IS NOT A VALID FORMAT SERIALIZER TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TFormatSerializer),
                arguments,
                loggerResolver);

            return (TFormatSerializer)Activator.CreateInstance(
                typeof(TFormatSerializer),
                 arguments);
            */
        }

        #endregion

        #region Data converters

        public InvokeMediumConverter BuildInvokeMediumConverter()
        {
            ILogger logger = loggerResolver?.GetLogger<InvokeMediumConverter>();

            return new InvokeMediumConverter(
                logger);
        }

        public ByteArrayFallbackConverter BuildByteArrayFallbackConverter(
            IDataConverter innerConverter,
            TypeDelegatePair[] convertFromBytesDelegates,
            TypeDelegatePair[] convertToBytesDelegates)
        {
            ILogger logger = loggerResolver?.GetLogger<ByteArrayFallbackConverter>();

            return new ByteArrayFallbackConverter(
                typeConversionFactory.BuildByteArrayConverter(
                    convertFromBytesDelegates,
                    convertToBytesDelegates),
                innerConverter,
                logger);
        }

        public TDataConverter BuildDataConverter<TDataConverter>(
            IDataConverter innerConverter,
            object[] arguments = null)
            where TDataConverter : IDataConverter
        {
            throw new NotImplementedException(
                $"BuildDataConverter<{typeof(TDataConverter).Name}> is not implemented yet.");

            /*
            if (Array.IndexOf(
                formatSerializerTypes,
                typeof(TDataConverter))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TDataConverter)} IS NOT A VALID DATA CONVERTER TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TDataConverter),
                arguments,
                loggerResolver);

            arguments = TryAppendDataConverter(
                typeof(TDataConverter),
                arguments,
                innerConverter);

            return (TDataConverter)Activator.CreateInstance(
                typeof(TDataConverter),
                 arguments);
            */
        }

        #endregion

        #region Serialization mediums

        public StringMedium BuildStringMedium(
            Func<string> valueGetter,
            Action<string> valueSetter)
        {
            return new StringMedium(
                valueGetter,
                valueSetter,
                loggerResolver?.GetLogger<StringMedium>());
        }

        public CachedStringMedium BuildCachedStringMedium()
        {
            return new CachedStringMedium(
                loggerResolver?.GetLogger<CachedStringMedium>());
        }

        public TextFileMedium BuildTextFileMedium(
            string fullPath)
        {
            return new TextFileMedium(
                fullPath,
                loggerResolver?.GetLogger<TextFileMedium>());
        }

        public BinaryFileMedium BuildBinaryFileMedium(
            string fullPath)
        {
            return new BinaryFileMedium(
                fullPath,
                loggerResolver?.GetLogger<BinaryFileMedium>());
        }

        public TextStreamMedium BuildTextStreamMedium(
            string fullPath,
            bool flushAutomatically = true)
        {
            return new TextStreamMedium(
                fullPath,
                loggerResolver?.GetLogger<TextStreamMedium>(),
                flushAutomatically);
        }

        public FileStreamMedium BuildFileStreamMedium(
            string fullPath,
            bool flushAutomatically = true)
        {
            return new FileStreamMedium(
                fullPath,
                loggerResolver?.GetLogger<FileStreamMedium>(),

                flushAutomatically);
        }

        public MemoryStreamMedium BuildMemoryStreamMedium(

            byte[] buffer = null,
            int index = -1,
            int count = -1)
        {
            return new MemoryStreamMedium(
                loggerResolver?.GetLogger<MemoryStreamMedium>(),

                buffer,
                index,
                count);
        }

        public IsolatedStorageMedium BuildIsolatedStorageMedium(
            string fullPath,

            bool flushAutomatically = true)
        {
            return new IsolatedStorageMedium(
                fullPath,
                loggerResolver?.GetLogger<IsolatedStorageMedium>(),
                
                flushAutomatically);
        }

        public TSerializationMedium BuildSerializationMedium<TSerializationMedium>(
            object[] arguments = null)
            where TSerializationMedium : ISerializationMedium
        {
            throw new NotImplementedException(
                $"BuildSerializationMedium<{typeof(TSerializationMedium).Name}> is not implemented yet.");

            /*
            if (Array.IndexOf(
                serializationMediumTypes,
                typeof(TSerializationMedium))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TSerializationMedium)} IS NOT A VALID SERIALIZATION MEDIUM TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TSerializationMedium),
                arguments,
                loggerResolver);

            return (TSerializationMedium)Activator.CreateInstance(
                typeof(TSerializationMedium),
                 arguments);
            */
        }

        #endregion

        #region Builder

        public SerializerBuilder BuildSerializerBuilder()
        {
            return new SerializerBuilder(
                this,
                metadataFactory,
                loggerResolver?.GetLogger<SerializerBuilder>());
        }

        #endregion

        #region Serializer

        public Serializer BuildSerializer(
            IReadOnlySerializerContext context)
        {
            return new Serializer(
                context,
                loggerResolver?.GetLogger<Serializer>());
        }

        public ConcurrentSerializer BuildConcurrentSerializer(
            IReadOnlySerializerContext context)
        {
            return new ConcurrentSerializer(
                new Serializer(
                    context,
                    loggerResolver?.GetLogger<Serializer>()),
                new SemaphoreSlim(1, 1));
        }

        #endregion

        private static object[] TryAppendLogger(
            Type type,
            object[] arguments,
            ILoggerResolver loggerResolver)
        {
            if (loggerResolver == null)
            {
                return arguments;
            }

            var constructor = type.GetConstructors()[0];

            var parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return arguments;
            }

            bool canReceiveLogger = parameters[parameters.Length - 1]
                .ParameterType
                .IsAssignableFrom(typeof(ILogger));

            if (!canReceiveLogger)
            {
                return arguments;
            }

            List<object> argumentList = new List<object>(arguments);

            argumentList.Add(
                loggerResolver.GetLogger(type));

            return argumentList.ToArray();
        }

        private static object[] TryAppendDataConverter(
            Type type,
            object[] arguments,
            IDataConverter innerDataConverter)
        {
            if (innerDataConverter == null)
            {
                return arguments;
            }

            var constructor = type.GetConstructors()[0];

            var parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return arguments;
            }

            bool canReceiveInnerConverter =
                parameters[parameters.Length - 2].ParameterType.IsAssignableFrom(typeof( IDataConverter));

            if (!canReceiveInnerConverter)
            {
                return arguments;
            }

            List<object> argumentList = new List<object>(arguments);

            argumentList.Insert(
                argumentList.Count - 1,
                innerDataConverter);

            return argumentList.ToArray();
        }
    }
}
