using System;

using HereticalSolutions.Metadata.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Builders
{
	public class SerializerBuilderContext
	{
		public MetadataFactory MetadataFactory;

		public ISerializerContext SerializerContext;

		public Action DeferredBuildFormatSerializerDelegate;

		public Action DeferredBuildDataConverterDelegate;

		public Action DeferredBuildSerializationMediumDelegate;

		public ILogger Logger;

		public void EnsureArgumentsExist()
		{
			if (SerializerContext.Arguments == null)
				SerializerContext.Arguments =
					MetadataFactory.BuildStronglyTypedMetadata();
		}
	}
}