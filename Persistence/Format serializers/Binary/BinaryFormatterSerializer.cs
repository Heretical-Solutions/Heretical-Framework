using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [FormatSerializer]
    public class BinaryFormatterSerializer
        : ABinarySerializer
    {
        private readonly BinaryFormatter formatter;

        public BinaryFormatterSerializer(
            BinaryFormatter formatter,
            ILogger logger)
            : base(
                logger)
        {
            this.formatter = formatter;
        }

        protected override bool CanSerializeWithStream => true;

        protected override bool CanDeserializeWithStream => true;

        protected override bool SerializeWithStream<TValue>(
            IMediumWithStream mediumWithStream,
            TValue value)
        {
            formatter.Serialize(
                mediumWithStream.Stream,
                value);

            return true;
        }

        protected override bool SerializeWithStream(
            IMediumWithStream mediumWithStream,
            Type valueType,
            object valueObject)
        {
            formatter.Serialize(
                mediumWithStream.Stream,
                valueObject);

            return true;
        }

        protected override bool DeserializeWithStream<TValue>(
            IMediumWithStream mediumWithStream,
            out TValue value)
        {
            var valueObject = formatter.Deserialize(
                mediumWithStream.Stream);

            value = valueObject.CastFromTo<object, TValue>();

            return true;
        }

        protected override bool DeserializeWithStream(
            IMediumWithStream mediumWithStream,
            Type valueType,
            out object valueObject)
        {
            valueObject = formatter.Deserialize(
                mediumWithStream.Stream);

            return true;
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override byte[] SerializeToByteArray<TValue>(
            TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(
                    memoryStream,
                    value);

                return memoryStream.ToArray();
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override byte[] SerializeToByteArray(
            Type valueType,
            object valueObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(
                    memoryStream,
                    valueObject);

                return memoryStream.ToArray();
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override bool DeserializeFromByteArray<TValue>(
            byte[] byteArrayValue,
            out TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(
                    byteArrayValue,
                    0,
                    byteArrayValue.Length);

                memoryStream.Seek(
                    0,
                    SeekOrigin.Begin);

                var valueObject = formatter.Deserialize(
                    memoryStream);

                value = valueObject.CastFromTo<object, TValue>();

                return true;
            }
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override bool DeserializeFromByteArray(
            byte[] byteArrayValue,
            Type valueType,
            out object valueObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(
                    byteArrayValue,
                    0,
                    byteArrayValue.Length);

                memoryStream.Seek(
                    0,
                    SeekOrigin.Begin);

                valueObject = formatter.Deserialize(
                    memoryStream);

                return true;
            }
        }
    }
}