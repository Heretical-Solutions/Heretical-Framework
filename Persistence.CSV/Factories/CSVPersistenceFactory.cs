#if CSV_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.CSV.Factories
{
	public class CSVPersistenceFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public CSVPersistenceFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public CSVSerializer BuildCSVSerializer(
			bool includeHeader,
			bool useFieldsInsteadOfProperties = true)
		{
			return new CSVSerializer(
				includeHeader,
				useFieldsInsteadOfProperties,
				loggerResolver?.GetLogger<CSVSerializer>());
		}
	}
}

#endif