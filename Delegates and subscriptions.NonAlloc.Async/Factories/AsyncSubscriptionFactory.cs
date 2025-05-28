using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Async.Factories
{
	public class AsyncSubscriptionFactory
	{
		private readonly TaskWrapperFactory taskWrapperFactory;

		private readonly ILoggerResolver loggerResolver;

		public AsyncSubscriptionFactory(
			TaskWrapperFactory taskWrapperFactory,
			ILoggerResolver loggerResolver)
		{
			this.taskWrapperFactory = taskWrapperFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Subscriptions

		public AsyncSubscriptionNoArgs
			BuildAsyncSubscriptionNoArgs(
				Func<AsyncExecutionContext, Task> taskFactory)
		{
			ILogger logger =
				loggerResolver?.GetLogger<SubscriptionNoArgs>();

			IAsyncInvokableNoArgs invokable = taskWrapperFactory
				.BuildTaskWrapperNoArgs(
					taskFactory);

			return new AsyncSubscriptionNoArgs(
				invokable,
				logger);
		}

		public AsyncSubscriptionSingleArgGeneric<TValue>
			BuildAsyncSubscriptionSingleArgGeneric<TValue>(
				Func<TValue, AsyncExecutionContext, Task> taskFactory)
		{
			ILogger logger =
				loggerResolver?.GetLogger<SubscriptionSingleArgGeneric<TValue>>();

			IAsyncInvokableSingleArgGeneric<TValue> invokable = taskWrapperFactory.
				BuildTaskWrapperSingleArgGeneric(
					taskFactory);

			return new AsyncSubscriptionSingleArgGeneric<TValue>(
				invokable,
				logger);
		}

		public AsyncSubscriptionMultipleArgs
			BuildAsyncSubscriptionMultipleArgs(
				Func<object[], AsyncExecutionContext, Task> taskFactory)
		{
			ILogger logger =
				loggerResolver?.GetLogger<SubscriptionMultipleArgs>();

			IAsyncInvokableMultipleArgs invokable = taskWrapperFactory.
				BuildTaskWrapperMultipleArgs(
					taskFactory);

			return new AsyncSubscriptionMultipleArgs(
				invokable,
				logger);
		}

		#endregion
	}
}