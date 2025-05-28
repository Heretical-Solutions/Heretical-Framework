#if CSV_SUPPORT

using System;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Persistence.CSV.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.CSV.Builders
{
	public static class SerializerBuilderExtensionsCSV
	{
		public static SerializerBuilder ToCSV(
			this SerializerBuilder builder,
			CSVPersistenceFactory csvPersistenceFactory,
			bool includeHeader = true)
		{
			var context = builder.Context;

			if (context.DeferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					context.Logger.TryFormatException(
						context.GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildFormatSerializerDelegate = () =>
			{
				if (context.SerializerContext.FormatSerializer != null)
				{
					throw new Exception(
						context.Logger.TryFormatException(
							context.GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.FormatSerializer =
					csvPersistenceFactory.BuildCSVSerializer(
						includeHeader);
			};

			return builder;
		}
	}
}

#endif