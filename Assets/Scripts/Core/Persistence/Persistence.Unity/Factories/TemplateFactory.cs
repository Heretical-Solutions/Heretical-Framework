using System;

using HereticalSolutions.Persistence.Serializers;
using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Persistence.Factories
{
    public static class UnityPersistenceFactory
    {
        public static BinarySerializer BuildSimpleUnityBinarySerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeBinaryIntoStreamStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeBinaryIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new BinarySerializer(strategyRepository);
        }
        
        public static ProtobufSerializer BuildSimpleUnityProtobufSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeProtobufIntoStreamStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeProtobufIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new ProtobufSerializer(strategyRepository);
        }
        
        public static JSONSerializer BuildSimpleUnityJSONSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeJsonIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeJsonIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeJsonIntoTextFileStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeJsonIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeJsonIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeJsonIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new JSONSerializer(strategyRepository);
        }

        public static XMLSerializer BuildSimpleUnityXMLSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeXmlIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeXmlIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeXmlIntoTextFileStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeXmlIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeXmlIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeXmlIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new XMLSerializer(strategyRepository);
        }
        
        public static YAMLSerializer BuildSimpleUnityYAMLSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeYamlIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeYamlIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeYamlIntoTextFileStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeYamlIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeYamlIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeYamlIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new YAMLSerializer(strategyRepository);
        }
        
        public static CSVSerializer BuildSimpleUnityCSVSerializer()
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeCsvIntoStringStrategy());
            
            database.Add(typeof(StreamArgument), new SerializeCsvIntoStreamStrategy());
            database.Add(typeof(TextFileArgument), new SerializeCsvIntoTextFileStrategy());
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeCsvIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeCsvIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeCsvIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new CSVSerializer(strategyRepository);
        }
    }
}