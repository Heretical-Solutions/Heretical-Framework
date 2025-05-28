using System;

namespace HereticalSolutions.Logging
{
	public interface ILoggerResolver
	{
		ILogger GetDefaultLogger();

		ILogger GetLogger<TLogSource>();

		ILogger GetLogger(
			Type logSourceType);
	}
}