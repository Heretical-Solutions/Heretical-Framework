using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public class TaskWrapperSingleArgGeneric<TValue>
		: IAsyncInvokableSingleArgGeneric<TValue>,
		  IAsyncInvokableSingleArg
	{
		private readonly Func<TValue, AsyncExecutionContext, Task> taskFactory;

		private readonly ILogger logger;

		public TaskWrapperSingleArgGeneric(
			Func<TValue, AsyncExecutionContext, Task> taskFactory,
			ILogger logger)
		{
			this.taskFactory = taskFactory;

			this.logger = logger;
		}

		#region IAsyncInvokableSingleArgGeneric

		public async Task InvokeAsync(
			TValue argument,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await taskFactory(
				argument,

				asyncContext);
		}

		public async Task InvokeAsync(
			object argument,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			switch (argument)
			{
				case TValue tValue:

					await taskFactory(
						tValue,

						asyncContext);

						break;
				
				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{argument.GetType().Name}\""));
			}
		}

		#endregion

		#region IAsyncInvokableSingleArg

		public Type ValueType => typeof(TValue);

		public async Task InvokeAsync<TArgument>(
			TArgument value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			switch (value)
			{
				case TValue tValue:

					await taskFactory(
						tValue,

						asyncContext);

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
		}

		public async Task InvokeAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			switch (value)
			{
				case TValue tValue:

					await taskFactory(
						tValue,
						
						asyncContext);

					break;

				default:

					throw new ArgumentException(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
		}

		#endregion
	}
}