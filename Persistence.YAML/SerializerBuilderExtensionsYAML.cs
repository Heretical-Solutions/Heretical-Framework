#if YAML_SUPPORT

using System;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Persistence.YAML.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.YAML.Builders
{
	public static class SerializerBuilderExtensionsYAML
	{
		public static SerializerBuilder ToYAML(
			this SerializerBuilder builder,
			YAMLPersistenceFactory yamlPersistenceFactory)
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
					yamlPersistenceFactory.BuildYAMLSerializer();
			};

			return builder;
		}
	}
}

#endif