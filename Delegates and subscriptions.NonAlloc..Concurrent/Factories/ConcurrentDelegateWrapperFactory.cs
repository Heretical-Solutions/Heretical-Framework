using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent.Factories
{
	public class ConcurrentDelegateWrapperFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public ConcurrentDelegateWrapperFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Concurrent delegate wrappers

		public ConcurrentDelegateWrapperNoArgs
			BuildConcurrentDelegateWrapperNoArgs(
				Action @delegate)
		{
			return new ConcurrentDelegateWrapperNoArgs(
				@delegate,
				new object());
		}

		public ConcurrentDelegateWrapperSingleArgGeneric<TValue>
			BuildConcurrentDelegateWrapperSingleArgGeneric<TValue>(
				Action<TValue> @delegate)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentDelegateWrapperSingleArgGeneric<TValue>>();

			return new ConcurrentDelegateWrapperSingleArgGeneric<TValue>(
				@delegate,
				new object(),
				logger);
		}

		public ConcurrentDelegateWrapperMultipleArgs
			BuildConcurrentDelegateWrapperMultipleArgs(
				Action<object[]> @delegate)
		{
			return new ConcurrentDelegateWrapperMultipleArgs(
				@delegate,
				new object());
		}

		#endregion
	}
}