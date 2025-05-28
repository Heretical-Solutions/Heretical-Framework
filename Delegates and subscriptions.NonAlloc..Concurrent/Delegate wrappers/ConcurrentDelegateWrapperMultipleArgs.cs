using System;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent
{
	public class ConcurrentDelegateWrapperMultipleArgs
		: IInvokableMultipleArgs
	{
		private readonly Action<object[]> @delegate;

		private readonly object lockObject;

		public ConcurrentDelegateWrapperMultipleArgs(
			Action<object[]> @delegate,
			object lockObject)
		{
			this.@delegate = @delegate;

			this.lockObject = lockObject;
		}

		public void Invoke(
			object[] arguments)
		{
			Action<object[]> copy;

			lock (lockObject)
			{
				copy = @delegate;  // Make a thread-safe copy of the delegate.
			}

			copy?.Invoke(arguments);  // Invoke the delegate outside of the lock.
		}
	}
}