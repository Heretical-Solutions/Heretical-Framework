#if XML_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.XML.Factories
{
	public class XMLPersistenceFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public XMLPersistenceFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public XMLSerializer BuildXMLSerializer()
		{
			return new XMLSerializer(
				loggerResolver?.GetLogger<XMLSerializer>());
		}
	}
}

#endif