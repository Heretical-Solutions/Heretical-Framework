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
	public class AsyncNonAllocBroadcasterGeneric<TValue>
		: IAsyncPublisherSingleArgGeneric<TValue>,
		  IAsyncPublisherSingleArg,
		  INonAllocSubscribable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IBag<INonAllocSubscription> subscriptionBag;

		private readonly IPool<IBag<INonAllocSubscription>> contextPool;

		private readonly object lockObject;

		private readonly ILogger logger;

		public AsyncNonAllocBroadcasterGeneric(
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
				switch (subscription)
				{
					case 
						INonAllocSubscriptionContext<IAsyncInvokableSingleArgGeneric<TValue>>
							singleArgGenericSubscriptionContext:

						if (!singleArgGenericSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionBag.Push(subscription))
							return false;

						singleArgGenericSubscriptionContext.Activate(this);

						break;

					case INonAllocSubscriptionContext<IAsyncInvokableSingleArg> 
						singleArgSubscriptionContext:

						if (!singleArgSubscriptionContext.ValidateActivation(this))
							return false;

						if (!subscriptionBag.Push(subscription))
							return false;

						singleArgSubscriptionContext.Activate(this);

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

						return false;
				}

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
				switch (subscription)
				{
					case 
						INonAllocSubscriptionContext<IAsyncInvokableSingleArgGeneric<TValue>>
							singleArgGenericSubscriptionContext:

						if (!singleArgGenericSubscriptionContext
							.ValidateTermination(this))
							return false;

						if (!subscriptionBag.Pop(subscription))
							return false;

						singleArgGenericSubscriptionContext.Terminate();

						break;

					case INonAllocSubscriptionContext<IAsyncInvokableSingleArg> 
						singleArgSubscriptionContext:

						if (!singleArgSubscriptionContext.ValidateTermination(this))
							return false;

						if (!subscriptionBag.Pop(subscription))
							return false;

						singleArgSubscriptionContext.Terminate();

						break;

					default:

						logger?.LogError(
							GetType(),
							$"INVALID SUBSCRIPTION TYPE: \"{subscription.GetType().Name}\"");

						return false;
				}

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

		#region IAsyncPublisherSingleArgGeneric

		public async Task PublishAsync(
			TValue value,

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
					as INonAllocSubscriptionContext<IAsyncInvokableSingleArgGeneric<TValue>>;

				if (subscriptionContext == null)
					continue;

				await subscriptionContext.Delegate?.InvokeAsync(
					value,
					asyncContext);
			}

			lock (lockObject)
			{
				//Clean up and push the context back into the pool
				context.Clear();

				contextPool.Push(context);
			}
		}

		#endregion

		#region IAsyncPublisherSingleArg

		public async Task PublishAsync<TArgument>(
			TArgument value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//lock (lockObject)
			//{
			switch (value)
			{
				case TValue tValue:

					await PublishAsync(
						tValue,
						
						asyncContext);

					break;

				default:

					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
			}
			//}
		}

		public async Task PublishAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//lock (lockObject)
			//{
			switch (value)
			{
				case TValue tValue:

					await PublishAsync(
						tValue,
						
						asyncContext);

					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
			}
			//}
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