using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Async.Factories
{
	public class TaskWrapperFactory
	{
		private readonly ILoggerResolver loggerResolver;

		public TaskWrapperFactory(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Task wrappers

		public TaskWrapperNoArgs BuildTaskWrapperNoArgs(
			Func<AsyncExecutionContext, Task> taskFactory)
		{
			return new TaskWrapperNoArgs(
				taskFactory);
		}

		public TaskWrapperSingleArgGeneric<TValue>
			BuildTaskWrapperSingleArgGeneric<TValue>(
				Func<TValue, AsyncExecutionContext, Task> taskFactory)
		{
			ILogger logger =
				loggerResolver?.GetLogger<TaskWrapperSingleArgGeneric<TValue>>();

			return new TaskWrapperSingleArgGeneric<TValue>(
				taskFactory,
				logger);
		}

		public TaskWrapperMultipleArgs BuildTaskWrapperMultipleArgs(
			Func<object[], AsyncExecutionContext, Task> taskFactory)
		{
			return new TaskWrapperMultipleArgs(
				taskFactory);
		}

		#endregion
	}
}