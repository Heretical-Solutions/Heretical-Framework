#if PROTOBUF_SUPPORT

using System;

using HereticalSolutions.Persistence.Builders;

using HereticalSolutions.Persistence.Protobuf.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Protobuf.Builders
{
	public static class SerializerBuilderExtensionsProtobuf
	{
		public static SerializerBuilder ToProtobuf(
			this SerializerBuilder builder,
			ProtobufPersistenceFactory protobufPersistenceFactory)
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
					protobufPersistenceFactory.BuildProtobufSerializer();
			};

			return builder;
		}
	}
}

#endif