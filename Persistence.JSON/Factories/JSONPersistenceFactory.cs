#if JSON_SUPPORT

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.JSON.Factories
{
	public class JSONPersistenceFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public JSONPersistenceFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public JSONSerializer BuildJSONSerializer()
		{
			return new JSONSerializer(
				loggerResolver?.GetLogger<JSONSerializer>());
		}
	}
}

#endif