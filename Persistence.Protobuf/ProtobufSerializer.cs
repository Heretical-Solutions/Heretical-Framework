#if PROTOBUF_SUPPORT

using System;
using System.IO;

using HereticalSolutions.Logging;

using ProtoBuf;
using ProtobufInternalSerializer = ProtoBuf.Serializer;

namespace HereticalSolutions.Persistence.Protobuf
{
    [FormatSerializer]
    public class ProtobufSerializer
        : ABinarySerializer
    {
        public ProtobufSerializer(
            ILogger logger)
            : base(
                logger)
        {
        }

        protected override bool CanSerializeWithStream => true;

        protected override bool CanDeserializeWithStream => true;

        protected override bool SerializeWithStream<TValue>(
            IMediumWithStream mediumWithStream,
            TValue value)
        {
            ProtobufInternalSerializer.Serialize<TValue>(
                mediumWithStream.Stream,
                value);

            return true;
        }

        protected override bool SerializeWithStream(
            IMediumWithStream mediumWithStream,
            Type valueType,
            object valueObject)
        {
            ProtobufInternalSerializer.NonGeneric.Serialize(
                mediumWithStream.Stream,
                valueObject);

            return true;
        }

        protected override bool DeserializeWithStream<TValue>(
            IMediumWithStream mediumWithStream,
            out TValue value)
        {
            value = ProtobufInternalSerializer.Deserialize<TValue>(
                mediumWithStream.Stream);

            return true;
        }

        protected override bool DeserializeWithStream(
            IMediumWithStream mediumWithStream,
            Type valueType,
            out object valueObject)
        {
            valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                valueType,
                mediumWithStream.Stream);

            return true;
        }

        //Courtesy of https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
        protected override byte[] SerializeToByteArray<TValue>(
            TValue value)
        {
            using (var memoryStream = new MemoryStream())
            {
                ProtobufInternalSerializer.Serialize<TValue>(
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
                ProtobufInternalSerializer.NonGeneric.Serialize(
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

                value = ProtobufInternalSerializer.Deserialize<TValue>(
                    memoryStream);

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

                valueObject = ProtobufInternalSerializer.NonGeneric.Deserialize(
                    valueType,
                    memoryStream);

                return true;
            }
        }
    }
}

#endif