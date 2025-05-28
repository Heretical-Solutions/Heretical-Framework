#if LZ4_SUPPORT

using System;

using HereticalSolutions.TypeConversion;

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence
{
	public class LZ4CompressionConverter
		: ACompressionConverter
	{
		private readonly LZ4Level compressionLevel;

		public LZ4CompressionConverter(
			IDataConverter innerDataConverter,
			ITypeConverter<byte[]> byteArrayConverter,
			LZ4Level compressionLevel,
			ILogger logger)
			: base(
				innerDataConverter,
				byteArrayConverter,
				logger)
		{
			this.compressionLevel = compressionLevel;
		}

		protected override bool Decompress(
			byte[] compressedData,
			out byte[] decompressedData)
		{
			// Get the original size (first 4 bytes)
			var originalSize = BitConverter.ToInt32(compressedData, 0);

			// Decompress the data
			decompressedData = new byte[originalSize];

			var actualSize = LZ4Codec.Decode(
				compressedData,
				4,
				compressedData.Length - 4,
				decompressedData,
				0,
				decompressedData.Length);

			if (actualSize != originalSize)
			{
				logger?.LogError(
					GetType(),
					$"DECOMPRESSED DATA SIZE ({actualSize}) DOES NOT MATCH ORIGINAL SIZE ({originalSize})");

				return false;
			}

			return true;
		}

		protected override bool Compress(
			byte[] dataToCompress,
			out byte[] compressedData)
		{
			// Calculate the worst-case compression size
			var maxCompressedLength = LZ4Codec.MaximumOutputSize(dataToCompress.Length);

			// Create buffer for compressed data (4 bytes for original size + compressed data)
			compressedData = new byte[4 + maxCompressedLength];

			// Store the original size in the first 4 bytes
			BitConverter.GetBytes(dataToCompress.Length).CopyTo(compressedData, 0);

			// Compress the data
			var compressedSize = LZ4Codec.Encode(
				dataToCompress,
				0,
				dataToCompress.Length,
				compressedData,
				4,
				maxCompressedLength,
				compressionLevel);

			if (compressedSize <= 0)
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			// Resize the array to actual size
			Array.Resize(ref compressedData, 4 + compressedSize);

			return true;
		}
	}
}

#endif