using System;
using System.Reflection;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class AFormatSerializer
		: IFormatSerializer,
		  IAsyncFormatSerializer
	{
		protected readonly ILogger logger;

		public AFormatSerializer(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region IFormatSerializer

		public abstract bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value);

		public abstract bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject);

		public abstract bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value);

		public abstract bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject);

		public virtual bool Populate<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			if (!Deserialize<TValue>(
				context,
				out var newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				value);

			return true;
		}

		public virtual bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			if (!Deserialize(
				valueType,
				context,
				out var newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				valueObject);

			return true;
		}

		#endregion

		#region IAsyncFormatSerializer

		#region Serialize

		public virtual async Task<bool> SerializeAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return Serialize<TValue>(
					context,
					value);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION"));
		}

		public virtual async Task<bool> SerializeAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				return Serialize(
					valueType,
					context,
					valueObject);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC SERIALIZATION"));
		}

		#endregion

		#region Deserialize

		public virtual async Task<(bool, TValue)> DeserializeAsync<TValue>(
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = Deserialize<TValue>(
					context,
					out TValue value);
				
				return (result, value);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION"));
		}

		public virtual async Task<(bool, object)> DeserializeAsync(
			Type valueType,
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = Deserialize(
					valueType,
					context,
					out object valueObject);
				
				return (result, valueObject);
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC DESERIALIZATION"));
		}

		#endregion

		#region Populate

		public virtual async Task<bool> PopulateAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = Populate<TValue>(
					context,
					value);
				
				return result;
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC POPULATION"));
		}

		public virtual async Task<bool> PopulateAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IFallbackToSyncArgument>())
			{
				bool result = Populate(
					valueType,
					context,
					valueObject);
				
				return result;
			}

			throw new InvalidOperationException(
				logger.FormatException(
					GetType(),
					$"THE FORMAT SERIALIZER {GetType().Name} DOES NOT SUPPORT ASYNC POPULATION"));
		}

		#endregion

		#endregion

		#region Try deserialize / serialize

		protected bool TryDeserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return TryDeserializeBlock(
					context,
					0,
					-1,
					out value);
			}

			return context.DataConverter.ReadAndConvert<TValue>(
				context,
				out value);
		}

		protected bool TryDeserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object value)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return TryDeserializeBlock(
					valueType,
					context,
					0,
					-1,
					out value);
			}

			return context.DataConverter.ReadAndConvert(
				valueType,
				context,
				out value);
		}

		protected bool TrySerialize<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return TrySerializeBlock(
					context,
					value,
					0,
					-1);
			}

			if (context.Arguments.Has<IAppendArgument>())
			{
				return context.DataConverter.ConvertAndAppend<TValue>(
					context,
					value);
			}

			return context.DataConverter.ConvertAndWrite<TValue>(
				context,
				value);
		}

		protected bool TrySerialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return TrySerializeBlock(
					valueType,
					context,
					valueObject,
					0,
					-1);
			}

			if (context.Arguments.Has<IAppendArgument>())
			{
				return context.DataConverter.ConvertAndAppend(
					valueType,
					context,
					valueObject);
			}

			return context.DataConverter.ConvertAndWrite(
				valueType,
				context,
				valueObject);
		}

		#endregion

		#region Try deserialize / serialize block

		protected bool TryDeserializeBlock<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			var blockDataConverter = context.DataConverter
				as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT A BLOCK DATA CONVERTER"));
			}

			return blockDataConverter.ReadBlockAndConvert<TValue>(
				context,
				blockOffset,
				blockSize,
				out value);
		}

		protected bool TryDeserializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			var blockDataConverter = context.DataConverter
				as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT A BLOCK DATA CONVERTER"));
			}

			return blockDataConverter.ReadBlockAndConvert(
				valueType,
				context,
				blockOffset,
				blockSize,
				out value);
		}

		protected bool TrySerializeBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			var blockDataConverter = context.DataConverter
				as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT A BLOCK DATA CONVERTER"));
			}

			return blockDataConverter.ConvertAndWriteBlock<TValue>(
				context,
				value,
				blockOffset,
				blockSize);
		}

		protected bool TrySerializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			var blockDataConverter = context.DataConverter
				as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT A BLOCK DATA CONVERTER"));
			}

			return blockDataConverter.ConvertAndWriteBlock(
				valueType,
				context,
				valueObject,
				blockOffset,
				blockSize);
		}

		#endregion

		#region Try deserialize / serialize async

		protected async Task<(bool, TValue)> TryDeserializeAsync<TValue>(
			ISerializationCommandContext context,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return await TryDeserializeBlockAsync<TValue>(
					context,
					0,
					-1,
					asyncContext);
			}

			var asyncDataConverter = context.DataConverter
				as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					var result = TryDeserialize<TValue>(
						context,
						out var value);
	
					return (result, value);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER {context.DataConverter} IS NOT AN ASYNC DATA CONVERTER"));
			}

			return await asyncDataConverter.ReadAsyncAndConvert<TValue>(
				context,
				asyncContext);
		}

		protected async Task<(bool, object)> TryDeserializeAsync(
			Type valueType,
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return await TryDeserializeBlockAsync(
					valueType,
					context,
					0,
					-1,
					asyncContext);
			}

			var asyncDataConverter = context.DataConverter
				as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					var result = TryDeserialize(
						valueType,
						context,
						out var value);
	
					return (result, value);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER {context.DataConverter} IS NOT AN ASYNC DATA CONVERTER"));
			}

			return await asyncDataConverter.ReadAsyncAndConvert(
				valueType,
				context,
				asyncContext);
		}

		protected async Task<bool> TrySerializeAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return await TrySerializeBlockAsync(
					context,
					value,
					0,
					-1,
					asyncContext);
			}

			var asyncDataConverter = context.DataConverter
				as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					return TrySerialize<TValue>(
						context,
						value);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER {context.DataConverter} IS NOT AN ASYNC DATA CONVERTER"));
			}

			if (context.Arguments.Has<IAppendArgument>())
			{
				return  await asyncDataConverter.ConvertAndAppendAsync<TValue>(
					context,
					value,
					asyncContext);
			}

			return await asyncDataConverter.ConvertAndWriteAsync<TValue>(
				context,
				value,
				asyncContext);
		}

		protected async Task<bool> TrySerializeAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (context.Arguments.Has<IBlockSerializationArgument>())
			{
				return await TrySerializeBlockAsync(
					valueType,
					context,
					valueObject,
					0,
					-1,
					asyncContext);
			}

			var asyncDataConverter = context.DataConverter
				as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					return TrySerialize(
						valueType,
						context,
						valueObject);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER {context.DataConverter} IS NOT AN ASYNC DATA CONVERTER"));
			}

			if (context.Arguments.Has<IAppendArgument>())
			{
				return await asyncDataConverter.ConvertAndAppendAsync(
					valueType,
					context,
					valueObject,
					asyncContext);
			}

			return await asyncDataConverter.ConvertAndWriteAsync(
				valueType,
				context,
				valueObject,
				asyncContext);
		}

		#endregion

		#region Try deserialize / serialize block async

		protected async Task<(bool, TValue)> TryDeserializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var asyncBlockDataConverter = context.DataConverter
				as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					var result = TryDeserializeBlock<TValue>(
						context,
						blockOffset,
						blockSize,
						out var value);
	
					return (result, value);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT AN ASYNC BLOCK DATA CONVERTER"));
			}

			return await asyncBlockDataConverter.ReadBlockAsyncAndConvert<TValue>(
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		protected async Task<(bool, object)> TryDeserializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var asyncBlockDataConverter = context.DataConverter
				as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					var result = TryDeserializeBlock(
						valueType,
						context,
						blockOffset,
						blockSize,
						out var value);
	
					return (result, value);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT AN ASYNC BLOCK DATA CONVERTER"));
			}

			return await asyncBlockDataConverter.ReadBlockAsyncAndConvert(
				valueType,
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		protected async Task<bool> TrySerializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var asyncBlockDataConverter = context.DataConverter
				as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					return TrySerializeBlock(
						context,
						value,
						blockOffset,
						blockSize);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT AN ASYNC BLOCK DATA CONVERTER"));
			}

			return await asyncBlockDataConverter.ConvertAndWriteBlockAsync<TValue>(
				context,
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		protected async Task<bool> TrySerializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var asyncBlockDataConverter = context.DataConverter
				as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					return TrySerializeBlock(
						valueType,
						context,
						valueObject,
						blockOffset,
						blockSize);
				}

				throw new InvalidOperationException(
					logger.FormatException(
						GetType(),
						$"THE DATA CONVERTER IS NOT AN ASYNC BLOCK DATA CONVERTER"));
			}

			return await asyncBlockDataConverter.ConvertAndWriteBlockAsync(
				valueType,
				context,
				valueObject,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion
	
		#region Populate with reflection

		protected void PopulateWithReflection(
			object source,
			object destination)
		{
			PopulateFieldsWithReflection(source, destination);

			PopulatePropertiesWithReflection(source, destination);
		}

		protected void PopulateFieldsWithReflection(
			object source,
			object destination)
		{
			var fields = source
				.GetType()
				.GetFields(
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			foreach (var field in fields)
			{
				var value = field.GetValue(source);

				field.SetValue(destination, value);
			}
		}

		protected void PopulatePropertiesWithReflection(
			object source,
			object destination)
		{
			PropertyInfo[] properties =
				source
				.GetType()
				.GetProperties(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

			foreach (var property in properties)
			{
				if (property != null
					&& property.CanWrite)
					property.SetValue(
						destination,
						property.GetValue(source, null),
						null);
			}
		}

		#endregion
	}
}