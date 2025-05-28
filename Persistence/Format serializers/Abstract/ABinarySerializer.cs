using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ABinarySerializer
		: AFormatSerializer,
		  IBlockFormatSerializer,
		  IAsyncFormatSerializer,
		  IAsyncBlockFormatSerializer
	{
		public ABinarySerializer(
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

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = SerializeWithStream<TValue>(
					mediumWithStream,
					value);

				if (result)
				{
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			return result;
		}

		public override bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = SerializeWithStream(
					mediumWithStream,
					valueType,
					valueObject);

				if (result)
				{
					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			return result;
		}

		public override bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value)
		{
			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeWithStream<TValue>(
					mediumWithStream,
					out value);

				if (result)
				{
					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				value = default(TValue);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out value);

			return result;
		}

		public override bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject)
		{
			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeWithStream(
					mediumWithStream,
					valueType,
					out valueObject);

				if (result)
				{
					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				valueObject = default(object);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out valueObject);

			return result;
		}

		public override bool Populate<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			bool result = false;

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeWithStream<TValue>(
					mediumWithStream,
					out var newValue1);

				if (result)
				{
					PopulateWithReflection(
						newValue1,
						value);

					return result;
				}
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
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

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeWithStream(
					mediumWithStream,
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

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
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

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = SerializeBlockWithStream<TValue>(
					mediumWithStream,
					value,
					blockOffset,
					blockSize);

				if (result)
				{
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = TrySerializeBlock<byte[]>(
				context,
				byteArrayValue,
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

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = SerializeBlockWithStream(
					mediumWithStream,
					valueType,
					valueObject,
					blockOffset,
					blockSize);

				if (result)
				{
					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = TrySerializeBlock<byte[]>(
				context,
				byteArrayValue,
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeBlockWithStream<TValue>(
					mediumWithStream,
					blockOffset,
					blockSize,
					out value);

				if (result)
				{
					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				value = default(TValue);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeBlockWithStream(
					mediumWithStream,
					valueType,
					blockOffset,
					blockSize,
					out valueObject);

				if (result)
				{
					return result;
				}
			}

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				valueObject = default(object);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeBlockWithStream<TValue>(
					mediumWithStream,
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

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = DeserializeBlockWithStream(
					mediumWithStream,
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

			if (!TryDeserializeBlock<byte[]>(
				context,
				blockOffset,
				blockSize,
				out byte[] byteArrayValue))
			{
				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
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

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await SerializeWithStreamAsync<TValue>(
					context,
					mediumWithStream,
					value,
					
					asyncContext);

				if (result)
				{
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = await TrySerializeAsync<byte[]>(
				context,
				byteArrayValue,
				
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

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await SerializeWithStreamAsync(
					context,
					mediumWithStream,
					valueType,
					valueObject,
					
					asyncContext);

				if (result)
				{
					return true;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = await TrySerializeAsync<byte[]>(
				context,
				byteArrayValue,
				
				asyncContext);

			return result;
		}

		public override async Task<(bool, TValue)> DeserializeAsync<TValue>(
			ISerializationCommandContext context,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			(bool, TValue) result = (false, default(TValue));

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await DeserializeWithStreamAsync<TValue>(
					context,
					mediumWithStream,

					asyncContext);

				if (result.Item1)
				{
					return result;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return result;
			}

			result.Item1 = DeserializeFromByteArray<TValue>(
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

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await DeserializeWithStreamAsync(
					context,
					mediumWithStream,
					valueType,
					
					asyncContext);

				if (result.Item1)
				{
					return result;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return result;
			}

			result.Item1 = DeserializeFromByteArray(
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

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				var streamResult = await DeserializeWithStreamAsync<TValue>(
					context,
					mediumWithStream,
					
					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						value);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return false;
			}

			result = DeserializeFromByteArray<TValue>(
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

			if (CanDeserializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				var streamResult = await DeserializeWithStreamAsync(
					context,
					mediumWithStream,
					valueType,
					
					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						valueObject);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeAsync<byte[]>(
				context,
				
				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return false;
			}

			result = DeserializeFromByteArray(
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

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await SerializeBlockWithStreamAsync<TValue>(
					context,
					mediumWithStream,
					value,
					blockOffset,
					blockSize,

					asyncContext);

				if (result)
				{
					return result;
				}
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			result = await TrySerializeBlockAsync<byte[]>(
				context,
				byteArrayValue,
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

			if (CanSerializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await SerializeBlockWithStreamAsync(
					context,
					mediumWithStream,
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

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			result = await TrySerializeBlockAsync<byte[]>(
				context,
				byteArrayValue,
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await DeserializeBlockWithStreamAsync<TValue>(
					context,
					mediumWithStream,
					blockOffset,
					blockSize,

					asyncContext);

				if (result.Item1)
				{
					return result;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
					blockOffset,
					blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return result;
			}

			result.Item1 = DeserializeFromByteArray<TValue>(
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				result = await DeserializeBlockWithStreamAsync(
					context,
					mediumWithStream,
					valueType,
					blockOffset,
					blockSize,

					asyncContext);

				if (result.Item1)
				{
					return result;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return result;
			}

			result.Item1 = DeserializeFromByteArray(
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				var streamResult = await DeserializeBlockWithStreamAsync<TValue>(
					context,
					mediumWithStream,
					blockOffset,
					blockSize,

					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						value);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return false;
			}

			result = DeserializeFromByteArray<TValue>(
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

			if (CanDeserializeBlockWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationMedium is IMediumWithStream mediumWithStream)
			{
				var streamResult = await DeserializeBlockWithStreamAsync(
					context,
					mediumWithStream,
					valueType,
					blockOffset,
					blockSize,

					asyncContext);

				if (streamResult.Item1)
				{
					PopulateWithReflection(
						streamResult.Item2,
						valueObject);

					return true;
				}
			}

			var byteArrayResult = await TryDeserializeBlockAsync<byte[]>(
				context,
				blockOffset,
				blockSize,

				asyncContext);

			if (!byteArrayResult.Item1)
			{
				return false;
			}

			result = DeserializeFromByteArray(
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

		#region Serialize / deserialize with stream

		protected virtual bool CanSerializeWithStream => false;

		protected virtual bool CanDeserializeWithStream => false;

		protected virtual bool CanSerializeBlockWithStream => false;

		protected virtual bool CanDeserializeBlockWithStream => false;

		#region Regular

		protected virtual bool SerializeWithStream<TValue>(
			IMediumWithStream mediumWithStream,
			TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeWithStream(
			IMediumWithStream mediumWithStream,
			Type valueType,
			object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream<TValue>(
			IMediumWithStream mediumWithStream,
			out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream(
			IMediumWithStream mediumWithStream,
			Type valueType,
			out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Async

		protected virtual async Task<bool> SerializeWithStreamAsync<TValue>(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return SerializeWithStream<TValue>(
					mediumWithStream,
					value);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
		}

		protected virtual async Task<bool> SerializeWithStreamAsync(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return SerializeWithStream(
					mediumWithStream,
					valueType,
					valueObject);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
		}

		protected virtual async Task<(bool, TValue)> DeserializeWithStreamAsync<TValue>(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = DeserializeWithStream<TValue>(
					mediumWithStream,
					out var value);
	
				return (result, value);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION WITH STREAM"));
		}

		protected virtual async Task<(bool, object)> DeserializeWithStreamAsync(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = DeserializeWithStream(
					mediumWithStream,
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

		protected virtual bool SerializeBlockWithStream<TValue>(
			IMediumWithStream mediumWithStream,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			throw new NotImplementedException();
		}

		protected virtual bool SerializeBlockWithStream(
			IMediumWithStream mediumWithStream,
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeBlockWithStream<TValue>(
			IMediumWithStream mediumWithStream,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeBlockWithStream(
			IMediumWithStream mediumWithStream,
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Async block

		protected virtual async Task<bool> SerializeBlockWithStreamAsync<TValue>(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return SerializeBlockWithStream<TValue>(
					mediumWithStream,
					value,
					blockOffset,
					blockSize);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION WITH STREAM"));
		}

		protected virtual async Task<bool> SerializeBlockWithStreamAsync(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return SerializeBlockWithStream(
					mediumWithStream,
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
			DeserializeBlockWithStreamAsync<TValue>(
				ISerializationCommandContext context,
				IMediumWithStream mediumWithStream,
				int blockOffset,
				int blockSize,
	
				//Async tail
				AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = DeserializeBlockWithStream<TValue>(
					mediumWithStream,
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

		protected virtual async Task<(bool, object)> DeserializeBlockWithStreamAsync(
			ISerializationCommandContext context,
			IMediumWithStream mediumWithStream,
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = DeserializeBlockWithStream(
					mediumWithStream,
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

		#region Serialize / deserialize to / from byte array

		protected abstract bool DeserializeFromByteArray<TValue>(
			byte[] byteArrayValue,
			out TValue value);

		protected abstract bool DeserializeFromByteArray(
			byte[] byteArrayValue,
			Type valueType,
			out object valueObject);

		protected abstract byte[] SerializeToByteArray<TValue>(
			TValue value);

		protected abstract byte[] SerializeToByteArray(
			Type valueType,
			object valueObject);

		#endregion
	}
}