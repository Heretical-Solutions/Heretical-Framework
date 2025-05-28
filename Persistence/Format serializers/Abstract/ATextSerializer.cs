using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    public abstract class ATextSerializer
		: AFormatSerializer,
          IBlockFormatSerializer,
          IAsyncFormatSerializer,
          IAsyncBlockFormatSerializer
    {
        public ATextSerializer(
            ILogger logger)
            : base(
                logger)
        {
        }

		#region IFormatSerializer

        public override bool Serialize<TValue>(
            ISerializationCommandContext context,
            TValue value)
        {
            bool result = false;

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = SerializeWithTextWriter<TValue>(
                    textStreamMedium,
                    value);

                if (result)
                {
                    return true;
                }
            }
            
            string stringValue = SerializeToString<TValue>(
                value);

            result = TrySerialize<string>(
                context,
                stringValue);

            return result;
        }

        public override bool Serialize(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject)
        {
            bool result = false;

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = SerializeWithTextWriter(
                    textStreamMedium,
                    valueType,
                    valueObject);

                if (result)
                {
                    return true;
                }
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = TrySerialize<string>(
                context,
                stringValue);

            return result;
        }

        public override bool Deserialize<TValue>(
            ISerializationCommandContext context,
            out TValue value)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamMedium,
                    out value);

                if (result)
                {
                    return result;
                }
            }
            
            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                value = default(TValue);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
                out value);

            return result;
        }

        public override bool Deserialize(
            Type valueType,
            ISerializationCommandContext context,
            out object valueObject)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeWithTextReader(
                    textStreamMedium,
                    valueType,
                    out valueObject);

                if (result)
                {
                    return result;
                }
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                valueObject = default(object);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
                valueType,
                out valueObject);

            return result;
        }

        public override bool Populate<TValue>(
            ISerializationCommandContext context,
            TValue value)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamMedium,
                    out var newValue1);

                if (result)
                {
                    PopulateWithReflection(
                        newValue1,
                        value);

                    return result;
                }
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
                out var newValue2);

            if (result)
            {
                PopulateWithReflection(
                    newValue2,
                    value);
            }

            return result;
        }

        public override bool Populate(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeWithTextReader(
                    textStreamMedium,
                    valueType,
                    out var newValueObject1);

                if (result)
                {
                    PopulateWithReflection(
                        newValueObject1,
                        valueObject);

                    return result;
                }
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                return false;
            }

            result = DeserializeFromString(
                stringValue,
                valueType,
                out var newValueObject2);

            if (result)
            {
                PopulateWithReflection(
                    newValueObject2,
                    valueObject);
            }

            return result;
        }

        #endregion

        #region IBlockFormatSerializer

        public virtual bool SerializeBlock<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = SerializeBlockWithTextWriter<TValue>(
                    textStreamMedium,
                    value,
                    blockOffset,
                    blockSize);

                if (result)
                {
                    return result;
                }
            }

            string stringValue = SerializeToString<TValue>(
                value);

            result = TrySerializeBlock<string>(
                context,
                stringValue,
                blockOffset,
                blockSize);

            return result;
        }

        public virtual bool SerializeBlock(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = SerializeBlockWithTextWriter(
                    textStreamMedium,
                    valueType,
                    valueObject,
                    blockOffset,
                    blockSize);

                if (result)
                {
                    return true;
                }
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = TrySerializeBlock<string>(
                context,
                stringValue,
                blockOffset,
                blockSize);

            return result;
        }

        public virtual bool DeserializeBlock<TValue>(
            ISerializationCommandContext context,
            int blockOffset,
            int blockSize,
            out TValue value)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeBlockWithTextReader<TValue>(
                    textStreamMedium,
                    blockOffset,
                    blockSize,
                    out value);

                if (result)
                {
                    return result;
                }
            }

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                value = default(TValue);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
                out value);

            return result;
        }

        public virtual bool DeserializeBlock(
            Type valueType,
            ISerializationCommandContext context,
            int blockOffset,
            int blockSize,
            out object valueObject)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeBlockWithTextReader(
                    textStreamMedium,
                    valueType,
                    blockOffset,
                    blockSize,
                    out valueObject);

                if (result)
                {
                    return result;
                }
            }

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                valueObject = default(object);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
                valueType,
                out valueObject);

            return result;
        }

        public virtual bool PopulateBlock<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeBlockWithTextReader<TValue>(
                    textStreamMedium,
                    blockOffset,
                    blockSize,
                    out var newValue1);

                if (result)
                {
                    PopulateWithReflection(
                        newValue1,
                        value);

                    return result;
                }
            }

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
                out var newValue2);

            if (result)
            {
                PopulateWithReflection(
                    newValue2,
                    value);
            }

            return result;
        }

        public virtual bool PopulateBlock(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = DeserializeBlockWithTextReader(
                    textStreamMedium,
                    valueType,
                    blockOffset,
                    blockSize,
                    out var newValueObject1);

                if (result)
                {
                    PopulateWithReflection(
                        newValueObject1,
                        valueObject);

                    return result;
                }
            }

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string stringValue))
            {
                return false;
            }

            result = DeserializeFromString(
                stringValue,
                valueType,
                out var newValueObject2);

            if (result)
            {
                PopulateWithReflection(
                    newValueObject2,
                    valueObject);
            }

            return result;
        }

        #endregion

        #region IAsyncFormatSerializer

        public override async Task<bool> SerializeAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanSerializeWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await SerializeWithTextWriterAsync<TValue>(
                    context,
                    textStreamMedium,
                    value,

                    asyncContext);

                if (result)
                {
                    return result;
                }
            }

            string stringValue = SerializeToString<TValue>(
                value);

            result = await TrySerializeAsync<string>(
                context,
                stringValue,

                asyncContext);

            return result;
        }

        public override async Task<bool> SerializeAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanSerializeWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await SerializeWithTextWriterAsync(
                    context,
                    textStreamMedium,
                    valueType,
                    valueObject,

                    asyncContext);

                if (result)
                {
                    return true;
                }
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = await TrySerializeAsync<string>(
                context,
                stringValue,

                asyncContext);

            return result;
        }

        public override async Task<(bool, TValue)> DeserializeAsync<TValue>(
            ISerializationCommandContext context,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            (bool, TValue) result = (false, default(TValue));

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await DeserializeWithTextReaderAsync<TValue>(
                    context,
                    textStreamMedium,

                    asyncContext);

                if (result.Item1)
                {
                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return result;
            }

            result.Item1 = DeserializeFromString<TValue>(
                byteArrayResult.Item2,
                out result.Item2);

            return result;
        }

        public override async Task<(bool, object)> DeserializeAsync(
            Type valueType,
            ISerializationCommandContext context,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            (bool, object) result = (false, default(object));

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await DeserializeWithTextReaderAsync(
                    context,
                    textStreamMedium,
                    valueType,

                    asyncContext);

                if (result.Item1)
                {
                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return result;
            }

            result.Item1 = DeserializeFromString(
                byteArrayResult.Item2,
                valueType,
                out result.Item2);

            return result;
        }

        public override async Task<bool> PopulateAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                var readerResult = await DeserializeWithTextReaderAsync<TValue>(
                    context,
                    textStreamMedium,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        value);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                byteArrayResult.Item2,
                out var newValue);

            if (result)
            {
                PopulateWithReflection(
                    newValue,
                    value);
            }

            return result;
        }

        public override async Task<bool> PopulateAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanDeserializeWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                var readerResult = await DeserializeWithTextReaderAsync(
                    context,
                    textStreamMedium,
                    valueType,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        valueObject);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            result = DeserializeFromString(
                byteArrayResult.Item2,
                valueType,
                out var newValue);

            if (result)
            {
                PopulateWithReflection(
                    newValue,
                    valueObject);
            }

            return result;
        }

        #endregion

        #region IAsyncBlockFormatSerializer

        public async Task<bool> SerializeBlockAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await SerializeBlockWithTextWriterAsync<TValue>(
                    context,
                    textStreamMedium,
                    value,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (result)
                {
                    return result;
                }
            }

            string stringValue = SerializeToString<TValue>(
                value);

            result = await TrySerializeBlockAsync<string>(
                context,
                stringValue,
                blockOffset,
                blockSize,

                asyncContext);

            return result;
        }

        public async Task<bool> SerializeBlockAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanSerializeBlockWithTextWriter
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await SerializeBlockWithTextWriterAsync(
                    context,
                    textStreamMedium,
                    valueType,
                    valueObject,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (result)
                {
                    return true;
                }
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            result = await TrySerializeBlockAsync<string>(
                context,
                stringValue,
                blockOffset,
                blockSize,

                asyncContext);

            return result;
        }

        public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
            ISerializationCommandContext context,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            (bool, TValue) result = (false, default(TValue));

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await DeserializeBlockWithTextReaderAsync<TValue>(
                    context,
                    textStreamMedium,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (result.Item1)
                {
                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                    blockOffset,
                    blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return result;
            }

            result.Item1 = DeserializeFromString<TValue>(
                byteArrayResult.Item2,
                out result.Item2);

            return result;
        }

        public async Task<(bool, object)> DeserializeBlockAsync(
            Type valueType,
            ISerializationCommandContext context,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            (bool, object) result = (false, default(object));

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                result = await DeserializeBlockWithTextReaderAsync(
                    context,
                    textStreamMedium,
                    valueType,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (result.Item1)
                {
                    return result;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                blockOffset,
                blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return result;
            }

            result.Item1 = DeserializeFromString(
                byteArrayResult.Item2,
                valueType,
                out result.Item2);

            return result;
        }

        public async Task<bool> PopulateBlockAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                var readerResult = await DeserializeBlockWithTextReaderAsync<TValue>(
                    context,
                    textStreamMedium,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        value);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                blockOffset,
                blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            result = DeserializeFromString<TValue>(
                byteArrayResult.Item2,
                out var newValue);

            if (result)
            {
                PopulateWithReflection(
                    newValue,
                    value);
            }

            return result;
        }

        public async Task<bool> PopulateBlockAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            if (CanDeserializeBlockWithTextReader
                && !context.Arguments.Has<IDataConversionArgument>()
                && context.SerializationMedium is TextStreamMedium textStreamMedium)
            {
                var readerResult = await DeserializeBlockWithTextReaderAsync(
                    context,
                    textStreamMedium,
                    valueType,
                    blockOffset,
                    blockSize,

                    asyncContext);

                if (readerResult.Item1)
                {
                    PopulateWithReflection(
                        readerResult.Item2,
                        valueObject);

                    return true;
                }
            }

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                blockOffset,
                blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            result = DeserializeFromString(
                byteArrayResult.Item2,
                valueType,
                out var newValue);

            if (result)
            {
                PopulateWithReflection(
                    newValue,
                    valueObject);
            }

            return result;
        }

        #endregion

        #region Serialize / deserialize with text reader / writer

        protected virtual bool CanSerializeWithTextWriter => false;

        protected virtual bool CanDeserializeWithTextReader => false;

        protected virtual bool CanSerializeBlockWithTextWriter => false;

        protected virtual bool CanDeserializeBlockWithTextReader => false;

        #region Regular

        protected virtual bool SerializeWithTextWriter<TValue>(
            TextStreamMedium textStreamMedium,
            TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeWithTextWriter(
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader<TValue>(
            TextStreamMedium textStreamMedium,
            out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader(
            TextStreamMedium textStreamMedium,
            Type valueType,
            out object valueObject)
		{
			throw new NotImplementedException();
		}

        #endregion

        #region Async

        protected virtual async Task<bool> SerializeWithTextWriterAsync<TValue>(
            ISerializationCommandContext context,
            TextStreamMedium textStreamMedium,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                return SerializeWithTextWriter<TValue>(
                    textStreamMedium,
                    value);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<bool> SerializeWithTextWriterAsync(
            ISerializationCommandContext context,
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                return SerializeWithTextWriter(
                    textStreamMedium,
                    valueType,
                    valueObject);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<(bool, TValue)>
            DeserializeWithTextReaderAsync<TValue>(
                ISerializationCommandContext context,
                TextStreamMedium textStreamMedium,
    
                //Async tail
                AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                bool result = DeserializeWithTextReader<TValue>(
                    textStreamMedium,
                    out var value);
    
                return (result, value);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<(bool, object)> DeserializeWithTextReaderAsync(
            ISerializationCommandContext context,
            TextStreamMedium textStreamMedium,
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                bool result = DeserializeWithTextReader(
                    textStreamMedium,
                    valueType,
                    out var valueObject);
    
                return (result, valueObject);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION WITH STREAM"));
        }

        #endregion

        #region Regular block

        protected virtual bool SerializeBlockWithTextWriter<TValue>(
            TextStreamMedium textStreamMedium,
            TValue value,
            int blockOffset,
            int blockSize)
        {
            throw new NotImplementedException();
        }

        protected virtual bool SerializeBlockWithTextWriter(
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject,
            int blockOffset,
            int blockSize)
        {
            throw new NotImplementedException();
        }

        protected virtual bool DeserializeBlockWithTextReader<TValue>(
            TextStreamMedium textStreamMedium,
            int blockOffset,
            int blockSize,
            out TValue value)
        {
            throw new NotImplementedException();
        }

        protected virtual bool DeserializeBlockWithTextReader(
            TextStreamMedium textStreamMedium,
            Type valueType,
            int blockOffset,
            int blockSize,
            out object valueObject)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Async block

        protected virtual async Task<bool> SerializeBlockWithTextWriterAsync<TValue>(
            ISerializationCommandContext context,
            TextStreamMedium textStreamMedium,
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                return SerializeBlockWithTextWriter<TValue>(
                    textStreamMedium,
                    value,
                    blockOffset,
                    blockSize);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<bool> SerializeBlockWithTextWriterAsync(
            ISerializationCommandContext context,
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                return SerializeBlockWithTextWriter(
                    textStreamMedium,
                    valueType,
                    valueObject,
                    blockOffset,
                    blockSize);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<(bool, TValue)> 
            DeserializeBlockWithTextReaderAsync<TValue>(
                ISerializationCommandContext context,
                TextStreamMedium textStreamMedium,
                int blockOffset,
                int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                bool result = DeserializeBlockWithTextReader<TValue>(
                    textStreamMedium,
                    blockOffset,
                    blockSize,
                    out var value);
    
                return (result, value);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION WITH STREAM"));
        }

        protected virtual async Task<(bool, object)>
            DeserializeBlockWithTextReaderAsync(
                ISerializationCommandContext context,
                TextStreamMedium textStreamMedium,
                Type valueType,
                int blockOffset,
                int blockSize,
    
                //Async tail
                AsyncExecutionContext asyncContext)
        {
            if (context.Arguments.Has<IFallbackToSyncArgument>())
            {
                bool result = DeserializeBlockWithTextReader(
                    textStreamMedium,
                    valueType,
                    blockOffset,
                    blockSize,
                    out var valueObject);
    
                return (result, valueObject);
            }

            throw new InvalidOperationException(
                logger.FormatException(
                    GetType(),
                    $"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION WITH STREAM"));
        }

        #endregion

        #endregion

        #region Serialize / deserialize to / from string

        protected abstract string SerializeToString<TValue>(
            TValue value);

		protected abstract string SerializeToString(
            Type valueType,
            object valueObject);

		protected abstract bool DeserializeFromString<TValue>(
            string stringValue,
            out TValue value);

		protected abstract bool DeserializeFromString(
            string stringValue,
            Type valueType,
            out object valueObject);

		#endregion
    }
}