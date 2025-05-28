using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class InvokeMediumConverter
		: IDataConverter,
		  IBlockDataConverter,
		  IAsyncDataConverter,
		  IAsyncBlockDataConverter
	{
		private readonly ILogger logger;

		public InvokeMediumConverter(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region IDataConverter

		#region Read

		public bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			return context.SerializationMedium.Read<TValue>(
				out value);
		}

		public bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			return context.SerializationMedium.Read(
				valueType,
				out value);
		}

		#endregion

		#region Write

		public bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return context.SerializationMedium.Write<TValue>(
				value);
		}

		public bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return context.SerializationMedium.Write(
				valueType,
				value);
		}

		#endregion

		#region Append

		public bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return context.SerializationMedium.Append<TValue>(
				value);
		}

		public bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return context.SerializationMedium.Append(
				valueType,
				value);
		}

		#endregion

		#endregion

		#region IBlockDataConverter

		#region Read

		public bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			IBlockSerializationMedium blockSerializationMedium =
				context.SerializationMedium as IBlockSerializationMedium;

			if (blockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT A BLOCK SERIALIZATION MEDIUM");

				value = default(TValue);

				return false;
			}

			return blockSerializationMedium.ReadBlock<TValue>(
				blockOffset,
				blockSize,
				out value);
		}

		public bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			IBlockSerializationMedium blockSerializationMedium =
				context.SerializationMedium as IBlockSerializationMedium;

			if (blockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT A BLOCK SERIALIZATION MEDIUM");

				value = default;

				return false;
			}

			return blockSerializationMedium.ReadBlock(
				valueType,
				blockOffset,
				blockSize,
				out value);
		}

		#endregion

		#region Write

		public bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			IBlockSerializationMedium blockSerializationMedium =
				context.SerializationMedium as IBlockSerializationMedium;

			if (blockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT A BLOCK SERIALIZATION MEDIUM");

				return false;
			}

			return blockSerializationMedium.WriteBlock<TValue>(
				value,
				blockOffset,
				blockSize);
		}

		public bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize)
		{
			IBlockSerializationMedium blockSerializationMedium =
				context.SerializationMedium as IBlockSerializationMedium;

			if (blockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT A BLOCK SERIALIZATION MEDIUM");

				return false;
			}

			return blockSerializationMedium.WriteBlock(
				valueType,
				value,
				blockOffset,
				blockSize);
		}

		#endregion

		#endregion

		#region IAsyncDataConverter

		#region Read

		public async Task<(bool, TValue)> ReadAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return (false, default(TValue));
			}

			return await asyncSerializationMedium.ReadAsync<TValue>(
				asyncContext);
		}

		public async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return (false, default);
			}

			return await asyncSerializationMedium.ReadAsync(
				valueType,
				asyncContext);
		}

		#endregion

		#region Write

		public async Task<bool> ConvertAndWriteAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncSerializationMedium.WriteAsync<TValue>(
				value,
				asyncContext);
		}

		public async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncSerializationMedium.WriteAsync(
				valueType,
				value,
				asyncContext);
		}

		#endregion

		#region Append

		public async Task<bool> ConvertAndAppendAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncSerializationMedium.AppendAsync<TValue>(
				value,
				asyncContext);
		}

		public async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationMedium asyncSerializationMedium =
				context.SerializationMedium as IAsyncSerializationMedium;

			if (asyncSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncSerializationMedium.AppendAsync(
				valueType,
				value,
				asyncContext);
		}

		#endregion

		#endregion

		#region IAsyncBlockDataConverter
		
		#region Read

		public async Task<(bool, TValue)> ReadBlockAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationMedium asyncBlockSerializationMedium =
				context.SerializationMedium as IAsyncBlockSerializationMedium;

			if (asyncBlockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION MEDIUM");

				return (false, default(TValue));
			}

			return await asyncBlockSerializationMedium.ReadBlockAsync<TValue>(
				blockOffset,
				blockSize,
				asyncContext);
		}

		public async Task<(bool, object)> ReadBlockAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationMedium asyncBlockSerializationMedium =
				context.SerializationMedium as IAsyncBlockSerializationMedium;

			if (asyncBlockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION MEDIUM");

				return (false, default);
			}

			return await asyncBlockSerializationMedium.ReadBlockAsync(
				valueType,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#region Write

		public async Task<bool> ConvertAndWriteBlockAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationMedium asyncBlockSerializationMedium =
				context.SerializationMedium as IAsyncBlockSerializationMedium;

			if (asyncBlockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncBlockSerializationMedium.WriteBlockAsync<TValue>(
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public async Task<bool> ConvertAndWriteBlockAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationMedium asyncBlockSerializationMedium =
				context.SerializationMedium as IAsyncBlockSerializationMedium;

			if (asyncBlockSerializationMedium == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationMedium.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION MEDIUM");

				return false;
			}

			return await asyncBlockSerializationMedium.WriteBlockAsync(
				valueType,
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion
	}
}