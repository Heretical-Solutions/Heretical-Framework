#if YAML_SUPPORT

using YamlDotNet.Serialization;

using YamlSerializerBuilder = YamlDotNet.Serialization.SerializerBuilder;
using YamlDeserializerBuilder = YamlDotNet.Serialization.DeserializerBuilder;

using YamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;
using YamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.YAML.Factories
{
	public class YAMLPersistenceFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public YAMLPersistenceFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public YAMLSerializer BuildYAMLSerializer()
		{
			return new YAMLSerializer(
				new YamlSerializerBuilder().Build(),
				new YamlDeserializerBuilder().Build(),
				loggerResolver?.GetLogger<YAMLSerializer>());
		}
	}
}

#endif