using HereticalSolutions.Systems.Async.Builders;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Systems.Async.Factories
{
	public class AsyncSystemFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public AsyncSystemFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public AsyncSystemBuilder BuildAsyncSystemBuilder()
		{
			return new AsyncSystemBuilder(
				loggerResolver?.GetLogger<AsyncSystemBuilder>());
		}
	}
}