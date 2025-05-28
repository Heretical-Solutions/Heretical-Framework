#if XML_SUPPORT

using System;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Persistence.XML.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.XML.Builders
{
	public static class SerializerBuilderExtensionsXML
	{
		public static SerializerBuilder ToXML(
			this SerializerBuilder builder,
			XMLPersistenceFactory xmlPersistenceFactory)
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
					xmlPersistenceFactory.BuildXMLSerializer();
			};

			return builder;
		}
	}
}

#endif