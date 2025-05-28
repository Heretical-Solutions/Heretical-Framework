#if LZ4_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;
using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsLZ4
	{
		public static SerializerBuilder WithLZ4Compression(
			this SerializerBuilder builder,
			LZ4PersistenceFactory lz4PersistenceFactory,
			LZ4Level compressionLevel)
		{
			builder.Context.DeferredBuildDataConverterDelegate += () =>
			{
				if (builder.Context.SerializerContext.DataConverter == null)
				{
					throw new Exception(
						builder.Context.Logger.TryFormatException(
							builder.Context.GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				builder.Context.SerializerContext.DataConverter =
					lz4PersistenceFactory.BuildLZ4CompressionConverter(
						builder.Context.SerializerContext.DataConverter,
						compressionLevel);
			};

			return builder;
		}
	}
}

#endif