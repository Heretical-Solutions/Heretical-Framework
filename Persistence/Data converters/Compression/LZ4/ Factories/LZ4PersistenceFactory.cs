#if LZ4_SUPPORT

using HereticalSolutions.TypeConversion.Factories;

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence.Factories
{
	public class LZ4PersistenceFactory
	{
		private readonly TypeConversionFactory typeConversionFactory;

		private readonly ILoggerResolver loggerResolver;

		public LZ4PersistenceFactory(
			TypeConversionFactory typeConversionFactory,
			ILoggerResolver loggerResolver)
		{
			this.typeConversionFactory = typeConversionFactory;

			this.loggerResolver = loggerResolver;
		}

		public LZ4CompressionConverter BuildLZ4CompressionConverter(
			IDataConverter innerDataConverter,
			LZ4Level compressionLevel)
		{
			var byteArrayConverter = typeConversionFactory.BuildByteArrayConverter(
				null,
				null);

			return new LZ4CompressionConverter(
				innerDataConverter,
				byteArrayConverter,
				compressionLevel,
				loggerResolver?.GetLogger<LZ4CompressionConverter>());
		}
	}
}

#endif