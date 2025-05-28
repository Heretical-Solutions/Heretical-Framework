#if YAML_SUPPORT

using System;
using System.Text;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

using YamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;
using YamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;

namespace HereticalSolutions.Persistence.YAML
{
    [FormatSerializer]
    public class YAMLSerializer
        : ATextSerializer
    {
        private readonly YamlDotNetSerializer yamlSerializer;

        private readonly YamlDotNetDeserializer yamlDeserializer;

        public YAMLSerializer(
            YamlDotNetSerializer yamlSerializer,
            YamlDotNetDeserializer yamlDeserializer,
            ILogger logger)
            : base(
                logger)
        {
            this.yamlSerializer = yamlSerializer;

            this.yamlDeserializer = yamlDeserializer;
        }

        protected override bool CanSerializeWithTextWriter => true;

        protected override bool CanDeserializeWithTextReader => true;

        protected override bool SerializeWithTextWriter<TValue>(
            TextStreamMedium textStreamMedium,
            TValue value)
        {
            yamlSerializer.Serialize(
                textStreamMedium.StreamWriter,
                value,
                typeof(TValue));

            return true;
        }

        protected override bool SerializeWithTextWriter(
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject)
        {
            yamlSerializer.Serialize(
                textStreamMedium.StreamWriter,
                valueObject,
                valueType);

            return true;
        }

        protected override bool DeserializeWithTextReader<TValue>(
            TextStreamMedium textStreamMedium,
            out TValue value)
        {
            value = yamlDeserializer.Deserialize<TValue>(
                textStreamMedium.StreamReader);

            return true;
        }

        protected override bool DeserializeWithTextReader(
            TextStreamMedium textStreamMedium,
            Type valueType,
            out object valueObject)
        {
            valueObject = yamlDeserializer.Deserialize(
                textStreamMedium.StreamReader,
                valueType);

            return true;
        }

        protected override string SerializeToString<TValue>(
            TValue value)
        {
            return yamlSerializer.Serialize(
                value,
                typeof(TValue));
        }

        protected override string SerializeToString(
            Type valueType,
            object valueObject)
        {
            return yamlSerializer.Serialize(
                valueObject,
                valueType);
        }

        protected override bool DeserializeFromString<TValue>(
            string stringValue,
            out TValue value)
        {
            value = yamlDeserializer.Deserialize<TValue>(
                stringValue);

            return true;
        }

        protected override bool DeserializeFromString(
            string stringValue,
            Type valueType,
            out object valueObject)
        {
            valueObject = yamlDeserializer.Deserialize(
                stringValue,
                valueType);

            return true;
        }
    }
}

#endif