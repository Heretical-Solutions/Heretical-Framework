using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent
{
	public class ConcurrentSubscriptionMultipleArgs
		: INonAllocSubscription,
		  INonAllocSubscriptionContext<IInvokableMultipleArgs>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly SubscriptionMultipleArgs innerSubscription;

		private readonly object lockObject;

		public ConcurrentSubscriptionMultipleArgs(
			SubscriptionMultipleArgs innerSubscription,
			object lockObject)
		{
			this.innerSubscription = innerSubscription;

			this.lockObject = lockObject;
		}

		#region INonAllocSubscription

		public bool Active
		{
			get
			{
				lock (lockObject)
				{
					return innerSubscription.Active;
				}
			}
		}

		public bool Subscribe(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				return innerSubscription.Subscribe(publisher);
			}
		}

		public bool Unsubscribe()
		{
			lock (lockObject)
			{
				return innerSubscription.Unsubscribe();
			}
		}


		#endregion

		#region INonAllocSubscriptionContext

		public IInvokableMultipleArgs Delegate
		{
			get
			{
				lock (lockObject)
				{
					return innerSubscription.Delegate;
				}
			}
		}

		public bool ValidateActivation(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				return innerSubscription.ValidateActivation(publisher);
			}
		}

		public void Activate(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				innerSubscription.Activate(publisher);
			}
		}

		public bool ValidateTermination(
			INonAllocSubscribable publisher)
		{
			lock (lockObject)
			{
				return innerSubscription.ValidateTermination(publisher);
			}
		}

		public void Terminate()
		{
			lock (lockObject)
			{
				innerSubscription.Terminate();
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				innerSubscription.Cleanup();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				innerSubscription.Dispose();
			}
		}

		#endregion
	}
}