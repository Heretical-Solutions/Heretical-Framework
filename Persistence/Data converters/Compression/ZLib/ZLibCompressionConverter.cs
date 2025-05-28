/*
#if ZLIB_SUPPORT

using System;
using System.IO;

using HereticalSolutions.TypeConversion;

using HereticalSolutions.Logging;

using ZlibCodec = Ionic.Zlib.ZlibCodec;
using CompressionMode = Ionic.Zlib.CompressionMode;
using FlushType = Ionic.Zlib.FlushType;
using ZlibConstants = Ionic.Zlib.ZlibConstants;

namespace HereticalSolutions.Persistence
{
	public class ZLibCompressionConverter
		: ACompressionConverter
	{
		public ZLibCompressionConverter(
			IDataConverter innerDataConverter,
			ITypeConverter<byte[]> byteArrayConverter,
			ILogger logger)
			: base(
				innerDataConverter,
				byteArrayConverter,
				logger)
		{
		}

		protected override bool Decompress(
			byte[] compressedData,
			out byte[] decompressedData)
		{
			try
			{
				// Get the original size (first 4 bytes)
				var originalSize = BitConverter.ToInt32(compressedData, 0);

				// Create a memory stream for the decompressed data
				using (var decompressedStream = new MemoryStream())
				{
					// Create a memory stream for the compressed data (skipping the first 4 bytes)
					using (var compressedStream =
						new MemoryStream(
							compressedData,
							4,
							compressedData.Length - 4))
					{
						// Decompress using Ionic.Zlib
						var buffer = new byte[4096];
						int read;
						
						// Use ZlibCodec for decompression
						var codec = new ZlibCodec(
							CompressionMode.Decompress);
						
						codec.InputBuffer = compressedStream.GetBuffer();
						codec.NextIn = 4;
						codec.AvailableBytesIn = (int)compressedStream.Length - 4;
						
						codec.OutputBuffer = buffer;
						
						while (codec.AvailableBytesIn > 0)
						{
							codec.NextOut = 0;
							codec.AvailableBytesOut = buffer.Length;
							
							var inflateStatus = codec.Inflate(FlushType.None);
							
							if (inflateStatus != ZlibConstants.Z_OK && inflateStatus != ZlibConstants.Z_STREAM_END)
							{
								throw new Exception($"Decompression error: {inflateStatus}");
							}
							
							decompressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
							
							if (inflateStatus == ZlibConstants.Z_STREAM_END)
								break;
						}
						
						// Finish decompression
						do
						{
							codec.NextOut = 0;
							codec.AvailableBytesOut = buffer.Length;
							
							var inflateStatus = codec.Inflate(FlushType.Finish);
							
							if (inflateStatus != ZlibConstants.Z_OK && inflateStatus != ZlibConstants.Z_STREAM_END)
							{
								throw new Exception($"Decompression error: {inflateStatus}");
							}
							
							decompressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
							
							if (inflateStatus == ZlibConstants.Z_STREAM_END)
								break;
							
						} while (true);
					}
					
					// Get the decompressed data
					decompressedData = decompressedStream.ToArray();
					
					// Verify the size
					if (decompressedData.Length != originalSize)
					{
						logger?.LogError(
							GetType(),
							$"DECOMPRESSED DATA SIZE ({decompressedData.Length}) DOES NOT MATCH ORIGINAL SIZE ({originalSize})");
						
						return false;
					}
					
					return true;
				}
			}
			catch (Exception ex)
			{
				logger?.LogError(
					GetType(),
					$"ERROR DECOMPRESSING DATA: {ex.Message}");
				
				decompressedData = null;
				return false;
			}
		}

		protected override bool Compress(
			byte[] dataToCompress,
			out byte[] compressedData)
		{
			try
			{
				// Create a memory stream for the compressed data (with 4 bytes at the beginning for original size)
				using (var compressedStream = new MemoryStream())
				{
					// Write the original size as the first 4 bytes
					compressedStream.Write(BitConverter.GetBytes(dataToCompress.Length), 0, 4);
					
					// Compress using Ionic.Zlib
					var codec = new ZlibCodec(
						CompressionMode.Compress);
					
					codec.InputBuffer = dataToCompress;
					codec.NextIn = 0;
					codec.AvailableBytesIn = dataToCompress.Length;
					
					var buffer = new byte[4096];
					codec.OutputBuffer = buffer;
					
					do
					{
						codec.NextOut = 0;
						codec.AvailableBytesOut = buffer.Length;
						
						var deflateStatus = codec.Deflate(codec.AvailableBytesIn == 0 ? FlushType.Finish : FlushType.None);
						
						if (deflateStatus != ZlibConstants.Z_OK && deflateStatus != ZlibConstants.Z_STREAM_END)
						{
							throw new Exception($"Compression error: {deflateStatus}");
						}
						
						compressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
						
						if (deflateStatus == ZlibConstants.Z_STREAM_END)
							break;
						
					} while (codec.AvailableBytesIn > 0 || codec.AvailableBytesOut == 0);
					
					// Get the compressed data
					compressedData = compressedStream.ToArray();
					
					return true;
				}
			}
			catch (Exception ex)
			{
				logger?.LogError(
					GetType(),
					$"ERROR COMPRESSING DATA: {ex.Message}");
				
				compressedData = null;
				return false;
			}
		}
	}
}

#endif
*/