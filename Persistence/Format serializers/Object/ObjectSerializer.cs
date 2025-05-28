using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[FormatSerializer]
	public class ObjectSerializer
		: AFormatSerializer,
		  IBlockFormatSerializer,
		  IAsyncFormatSerializer,
		  IAsyncBlockFormatSerializer
	{
		public ObjectSerializer(
			ILogger logger)
			: base (
				logger)
		{
		}

		#region IFormatSerializer

		public override bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			var result = TrySerialize<TValue>(
				context,
				value);

			return result;
		}

		public override bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			var result = TrySerialize(
				valueType,
				context,
				valueObject);

			return result;
		}

		public override bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value)
		{
			if (!TryDeserialize<TValue>(
				context,
				out value))
			{
				value = default(TValue);

				return false;
			}

			return true;
		}

		public override bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject)
		{
			if (!TryDeserialize(
				valueType,
				context,
				out valueObject))
			{
				valueObject = default(object);

				return false;
			}

			return true;
		}

		public override bool Populate<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			if (!TryDeserialize<TValue>(
				context,
				out TValue newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				value);

			return true;
		}

		public override bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			if (!TryDeserialize(
				valueType,
				context,
				out object newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				valueObject);

			return true;
		}

		#endregion

		#region IBlockFormatSerializer

		#region Serialize

		public bool SerializeBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			return TrySerializeBlock<TValue>(
				context,
				value,
				blockOffset,
				blockSize);
		}

		public bool SerializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			return TrySerializeBlock(
				valueType,
				context,
				valueObject,
				blockOffset,
				blockSize);
		}

		#endregion

		#region Deserialize

		public bool DeserializeBlock<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			if (!TryDeserializeBlock<TValue>(
				context,
				blockOffset,
				blockSize,
				out value))
			{
				value = default(TValue);

				return false;
			}

			return true;
		}

		public bool DeserializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			if (!TryDeserializeBlock(
				valueType,
				context,
				blockOffset,
				blockSize,
				out valueObject))
			{
				valueObject = default(object);

				return false;
			}

			return true;
		}

		#endregion

		#region Populate

		public bool PopulateBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			if (!TryDeserializeBlock<TValue>(
				context,
				blockOffset,
				blockSize,
				out TValue newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				value);

			return true;
		}

		public bool PopulateBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			if (!TryDeserializeBlock(
				valueType,
				context,
				blockOffset,
				blockSize,
				out object newValue))
			{
				return false;
			}

			PopulateWithReflection(
				newValue,
				valueObject);

			return true;
		}

		#endregion

		#endregion

		#region IAsyncFormatSerializer
	
		#region Serialize

		public override async Task<bool> SerializeAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TrySerializeAsync<TValue>(
				context,
				value,
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
			var result = await TrySerializeAsync(
				valueType,
				context,
				valueObject,
				asyncContext);

			return result;
		}

		#endregion

		#region Deserialize

		public override async Task<(bool, TValue)> DeserializeAsync<TValue>(
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TryDeserializeAsync<TValue>(
				context,
				asyncContext);

			return result;
		}

		public override async Task<(bool, object)> DeserializeAsync(
			Type valueType,
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TryDeserializeAsync(
				valueType,
				context,
				asyncContext);

			return result;
		}

		#endregion

		#region Populate

		public override async Task<bool> PopulateAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TryDeserializeAsync<TValue>(
				context,
				asyncContext);

			if (!result.Item1)
			{
				return false;
			}

			PopulateWithReflection(
				result.Item2,
				value);

			return true;
		}

		public override async Task<bool> PopulateAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TryDeserializeAsync(
				valueType,
				context,
				asyncContext);

			if (!result.Item1)
			{
				return false;
			}

			PopulateWithReflection(
				result.Item2,
				valueObject);

			return true;
		}

		#endregion

		#endregion

		#region IAsyncBlockFormatSerializer
	
		#region Serialize

		public async Task<bool> SerializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await TrySerializeBlockAsync<TValue>(
				context,
				value,
				blockOffset,
				blockSize,
				asyncContext);
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
			return await TrySerializeBlockAsync(
				valueType,
				context,
				valueObject,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await TryDeserializeBlockAsync<TValue>(
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public async Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await TryDeserializeBlockAsync(
				valueType,
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = await TryDeserializeBlockAsync<TValue>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (!result.Item1)
			{
				return false;
			}

			PopulateWithReflection(
				result.Item2,
				value);

			return true;
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
			var result = await TryDeserializeBlockAsync(
				valueType,
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (!result.Item1)
			{
				return false;
			}

			PopulateWithReflection(
				result.Item2,
				valueObject);

			return true;
		}

		#endregion

		#endregion
	}
}