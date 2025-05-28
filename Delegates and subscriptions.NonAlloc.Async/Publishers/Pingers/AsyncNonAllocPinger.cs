using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Bags;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Async
{
	public class AsyncNonAllocPinger
		: IAsyncPublisherNoArgs,
		  INonAllocSubscribable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IBag<INonAllocSubscription> subscriptionBag;

		private readonly IPool<IBag<INonAllocSubscription>> contextPool;

		private readonly object lockObject;

		private readonly ILogger logger;

		public AsyncNonAllocPinger(
			IBag<INonAllocSubscription> subscriptionBag,
			IPool<IBag<INonAllocSubscription>> contextPool,
			object lockObject,
			ILogger logger)
		{
			this.subscriptionBag = subscriptionBag;

			this.contextPool = contextPool;

			this.lockObject = lockObject;

			this.logger = logger;
		}

		#region INonAllocSubscribable

		public bool Subscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				var subscriptionContext = subscription
					as INonAllocSubscriptionContext<IAsyncInvokableNoArgs>;
	
				if (subscriptionContext == null)
					return false;
	
				if (!subscriptionContext.ValidateActivation(this))
					return false;
	
				if (!subscriptionBag.Push(
					subscription))
					return false;
	
				subscriptionContext.Activate(
					this);
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION {subscription.GetHashCode()} ADDED: {this.GetHashCode()}");
	
				return true;
			}
		}

		public bool Unsubscribe(
			INonAllocSubscription subscription)
		{
			lock (lockObject)
			{
				var subscriptionContext = subscription
					as INonAllocSubscriptionContext<IAsyncInvokableNoArgs>;
	
				if (subscriptionContext == null)
					return false;
	
				if (!subscriptionContext.ValidateTermination(this))
					return false;
	
				if (!subscriptionBag.Pop(
					subscription))
					return false;
	
				subscriptionContext.Terminate();
	
				logger?.Log(
					GetType(),
					$"SUBSCRIPTION {subscription.GetHashCode()} REMOVED: {this.GetHashCode()}");
	
				return true;
			}
		}

		public IEnumerable<INonAllocSubscription> AllSubscriptions
		{
			get
			{
				lock (lockObject)
				{
					return subscriptionBag.All;
				}
			}
		}

		public void UnsubscribeAll()
		{
			lock (lockObject)
			{
				//foreach (var subscription in subscriptionBag.All)
				//{
				//	Unsubscribe(subscription);
				//}

				while (subscriptionBag.Count > 0)
				{
					if (!subscriptionBag.Peek(out var subscription))
						break;

					Unsubscribe(subscription);
				}

				subscriptionBag.Clear();
			}
		}

		#endregion

		#region IAsyncPublisherNoArgs

		public async Task PublishAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IBag<INonAllocSubscription> context = null;

			lock (lockObject)
			{
				if (subscriptionBag.Count == 0)
					return;

				// Pop context out of the pool and initialize it with values from the bag

				context = contextPool.Pop();

				context.Clear();

				foreach (var subscription in subscriptionBag.All)
				{
					context.Push(subscription);
				}
			}

			// Invoke the delegates in the context

			foreach (var subscription in context.All)
			{
				if (subscription == null)
					continue;

				if (!subscription.Active)
					continue;

				var subscriptionContext = subscription
					as INonAllocSubscriptionContext<IAsyncInvokableNoArgs>;

				if (subscriptionContext == null)
					continue;

				await subscriptionContext.Delegate?.InvokeAsync(asyncContext);
			}

			lock (lockObject)
			{
				//Clean up and push the context back into the pool
				context.Clear();

				contextPool.Push(context);
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				UnsubscribeAll();

				if (subscriptionBag is ICleanuppable)
					(subscriptionBag as ICleanuppable).Cleanup();
	
				if (contextPool is ICleanuppable)
					(contextPool as ICleanuppable).Cleanup();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				UnsubscribeAll();

				if (subscriptionBag is IDisposable)
					(subscriptionBag as IDisposable).Dispose();
	
				if (contextPool is IDisposable)
					(contextPool as IDisposable).Dispose();
			}
		}

		#endregion
	}
}