using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging.Builders
{
	public class LoggerBuilderContext
	{
		public IRepository<Type, bool> ExplicitLogSourceRules;

		public ILogger RootLogger;

		public ILogger CurrentLogger;

		public bool AllowedByDefault;
	}
}