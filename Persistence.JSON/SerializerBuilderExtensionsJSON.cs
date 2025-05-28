#if JSON_SUPPORT

using System;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Persistence.JSON.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.JSON.Builders
{
	public static class SerializerBuilderExtensionsJSON
	{
		public static SerializerBuilder ToJSON(
			this SerializerBuilder builder,
			JSONPersistenceFactory jsonPersistenceFactory)
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
					jsonPersistenceFactory.BuildJSONSerializer();
			};

			return builder;
		}
	}
}

#endif