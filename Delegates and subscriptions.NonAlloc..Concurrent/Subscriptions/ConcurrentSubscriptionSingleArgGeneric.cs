using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent
{
	public class ConcurrentSubscriptionSingleArgGeneric<TValue>
		: INonAllocSubscription,
		  INonAllocSubscriptionContext<IInvokableSingleArg>,
		  INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly SubscriptionSingleArgGeneric<TValue> innerSubscription;

		private readonly object lockObject;

		public ConcurrentSubscriptionSingleArgGeneric(
			SubscriptionSingleArgGeneric<TValue> innerSubscription,
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
				lock (this)
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

		IInvokableSingleArg INonAllocSubscriptionContext<IInvokableSingleArg>.Delegate
		{
			get
			{
				lock (lockObject)
				{
					var subscriptionContext = innerSubscription
						as INonAllocSubscriptionContext<IInvokableSingleArg>;

					return subscriptionContext.Delegate;
				}
			}
		}

		IInvokableSingleArgGeneric<TValue> INonAllocSubscriptionContext<IInvokableSingleArgGeneric<TValue>>.Delegate
		{
			get
			{
				lock (lockObject)
				{
					var subscriptionContext = innerSubscription
						as INonAllocSubscriptionContext<
							IInvokableSingleArgGeneric<TValue>>;

					return subscriptionContext.Delegate;
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