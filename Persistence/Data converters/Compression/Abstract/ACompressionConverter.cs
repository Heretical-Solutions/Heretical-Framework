#if LZ4_SUPPORT

using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.TypeConversion;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ACompressionConverter
		: AWrapperConverter
	{
		protected readonly ITypeConverter<byte[]> byteArrayConverter;

		public ACompressionConverter(
			IDataConverter innerDataConverter,
			ITypeConverter<byte[]> byteArrayConverter,
			ILogger logger)
			: base(
				innerDataConverter,
				logger)
		{
			this.byteArrayConverter = byteArrayConverter;
		}

		#region IDataConverter

		#region Read

		public override bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromTargetType<TValue>(
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		public override bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromTargetType(
					valueType,
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		#endregion

		#region Write

		public override bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				compressedData);
		}

		public override bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				compressedData);
		}

		#endregion

		#region Append

		public override bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				compressedData);
		}

		public override bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				compressedData);
		}

		#endregion

		#endregion

		#region IBlockDataConverter

		#region Read

		public override bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromTargetType<TValue>(
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		public override bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromTargetType(
					valueType,
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		#endregion

		#region Write

		public override bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize);
		}

		public override bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize);
		}

		#endregion

		#endregion

		#region IAsyncDataConverter

		#region Read

		public override async Task<(bool, TValue)> ReadAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromTargetType<TValue>(
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {typeof(TValue).Name}");

					return (false, default(TValue));
				}
			}

			return (result.Item1, value);
		}

		public override async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromTargetType(
					valueType,
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}
			}

			return (result.Item1, value);
		}

		#endregion

		#region Write

		public override async Task<bool> ConvertAndWriteAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		public override async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		#endregion

		#region Append

		public override async Task<bool> ConvertAndAppendAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		public override async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		#endregion

		#endregion

		#region IAsyncBlockDataConverter

		#region Read

		public override async Task<(bool, TValue)> ReadBlockAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromTargetType<TValue>(
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {typeof(TValue).Name}");

					return (false, default(TValue));
				}
			}

			return (result.Item1, value);
		}

		public override async Task<(bool, object)> ReadBlockAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromTargetType(
					valueType,
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}
			}

			return (result.Item1, value);
		}

		#endregion

		#region Write

		public override async Task<bool> ConvertAndWriteBlockAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public override async Task<bool> ConvertAndWriteBlockAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToTargetType(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion

		protected abstract bool Decompress(
			byte[] compressedData,
			out byte[] decompressedData);

		protected abstract bool Compress(
			byte[] dataToCompress,
			out byte[] compressedData);
	}
}

#endif