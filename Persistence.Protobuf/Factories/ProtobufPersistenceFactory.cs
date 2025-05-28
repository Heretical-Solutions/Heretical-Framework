#if PROTOBUF_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Protobuf.Factories
{
	public class ProtobufPersistenceFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ProtobufPersistenceFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public ProtobufSerializer BuildProtobufSerializer()
		{
			return new ProtobufSerializer(
				loggerResolver?.GetLogger<ProtobufSerializer>());
		}
	}
}

#endif