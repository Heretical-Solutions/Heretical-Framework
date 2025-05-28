using HereticalSolutions.Systems.Delegate.Builders;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Systems.Delegate.Factories
{
	public class DelegateSystemFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public DelegateSystemFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public DelegateSystemBuilder BuildDelegateSystemBuilder()
		{
			return new DelegateSystemBuilder(
				loggerResolver?.GetLogger<DelegateSystemBuilder>());
		}
	}
}