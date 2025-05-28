using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Logging
{
	public class LoggerResolver
		: ILoggerResolver
	{
		private readonly ILogger rootLogger;

		private readonly IRepository<Type, bool> explicitLogSourceRules;

		private readonly bool allowedByDefault;

		public LoggerResolver(
			ILogger rootLogger,
			IRepository<Type, bool> explicitLogSourceRules,
			bool allowedByDefault)
		{
			this.rootLogger = rootLogger;

			this.explicitLogSourceRules = explicitLogSourceRules;

			this.allowedByDefault = allowedByDefault;
		}

		#region ILoggerResolver

		public ILogger GetDefaultLogger()
		{
			return rootLogger;
		}

		public ILogger GetLogger<TLogSource>()
		{
			return GetLogger(
				typeof(TLogSource));
		}

		public ILogger GetLogger(
			Type logSourceType)
		{
			bool provide = allowedByDefault;

			if (explicitLogSourceRules.TryGetByTypeOrInheritor(
				logSourceType,
				out var explicitLogSourceRule))
			{
				provide = explicitLogSourceRule;
			}

			if (provide)
			{
				return rootLogger;
			}

			return null;
		}

		#endregion
	}
}