using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Messaging.Concurrent
{
	public class MainThreadCommand
	{
		public ECommandStatus Status
		{
			get
			{
				ECommandStatus result;

				lock (lockObject)
				{
					result = status;
				}

				return result;
			}
		}

		public Action DelegateToPerform { get; private set; }

		public Func<Task> AsyncDelegateToPerform { get; private set; }

		public bool Async { get; private set; }


		private ECommandStatus status;

		private object lockObject;


		public MainThreadCommand(
			Action delegateToPerform,
			object lockObject)
		{
			status = ECommandStatus.QUEUED;

			DelegateToPerform = delegateToPerform;

			this.lockObject = lockObject;

			AsyncDelegateToPerform = null;

			Async = false;
		}

		public MainThreadCommand(
			Func<Task> asyncDelegateToPerform)
		{
			status = ECommandStatus.QUEUED;

			AsyncDelegateToPerform = asyncDelegateToPerform;

			DelegateToPerform = null;

			Async = true;
		}

		public void Execute()
		{
			lock (lockObject)
			{
				status = ECommandStatus.IN_PROGRESS;
			}

			DelegateToPerform?.Invoke();

			lock (lockObject)
			{
				status = ECommandStatus.DONE;
			}
		}

		public async Task ExecuteAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			lock (lockObject)
			{
				status = ECommandStatus.IN_PROGRESS;
			}

			//TODO: .ThrowExceptionsIfAny() here
			await AsyncDelegateToPerform?
				.Invoke();

			lock (lockObject)
			{
				status = ECommandStatus.DONE;
			}
		}
	}
}