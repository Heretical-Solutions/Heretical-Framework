/*
#if ZLIB_SUPPORT

using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

using Ionic.Zlib;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsZLib
	{
		public static SerializerBuilder WithZLibCompression(
			this SerializerBuilder builder)
		{
			var builderCasted = builder as SerializerBuilderInternal;

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
					ZLibPersistenceFactory.BuildZLibCompressionConverter(
						builder.Context.SerializerContext.DataConverter,
						builder.Context.LoggerResolver);
			};

			return builder;
		}
	}
}

#endif
*/