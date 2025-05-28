using System;
using System.Threading;
using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Async.Factories
{
	public partial class NotifierFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public NotifierFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		public AsyncNotifierSingleArgGeneric<TArgument, TValue>
			BuildAsyncNotifierSingleArgGeneric<TArgument, TValue>()
			where TArgument : IEquatable<TArgument>
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncNotifierSingleArgGeneric<TArgument, TValue>>();

			return new AsyncNotifierSingleArgGeneric<TArgument, TValue>(
				new List<NotifyRequestSingleArgGeneric<TArgument, TValue>>(),
				new SemaphoreSlim(1, 1),
				logger);
		}
	}
}