using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class NonAllocBroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          INonAllocSubscribableMultipleArgs
    {
        #region Subscriptions
		
		private readonly INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool;

		private readonly IIndexable<IPoolElement<IInvokableMultipleArgs>> subscriptionsAsIndexable;

		private readonly IFixedSizeCollection<IPoolElement<IInvokableMultipleArgs>> subscriptionsWithCapacity;

		#endregion
		
		#region Buffer

		private IInvokableMultipleArgs[] currentSubscriptionsBuffer;

		private int currentSubscriptionsBufferCount = -1;

		#endregion
		
		private bool broadcastInProgress = false;

		public NonAllocBroadcasterMultipleArgs(
			INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool,
			INonAllocPool<IInvokableMultipleArgs> subscriptionsContents)
		{
			this.subscriptionsPool = subscriptionsPool;

			subscriptionsAsIndexable = (IIndexable<IPoolElement<IInvokableMultipleArgs>>)subscriptionsContents;

			subscriptionsWithCapacity =
				(IFixedSizeCollection<IPoolElement<IInvokableMultipleArgs>>)subscriptionsContents;

			currentSubscriptionsBuffer = new IInvokableMultipleArgs[subscriptionsWithCapacity.Capacity];
		}

		#region IPublisherMultipleArgs
		
		public void Subscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)
		{
			if (!subscription.ValidateActivation(this))
				return;
			
			var subscriptionElement = subscriptionsPool.Pop(null);

			subscriptionElement.Value = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).Invokable;

			subscription.Activate(this, subscriptionElement);
		}

		public void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs> subscription)
		{
			if (!subscription.ValidateTermination(this))
				return;

			var poolElement = ((ISubscriptionState<IInvokableMultipleArgs>)subscription).PoolElement;
			
			TryRemoveFromBuffer(poolElement);
			
			poolElement.Value = null;

			subscriptionsPool.Push(poolElement);
			
			subscription.Terminate();
		}

		public void Unsubscribe(IPoolElement<IInvokableMultipleArgs> subscription)
		{
			TryRemoveFromBuffer(subscription);
			
			subscription.Value = null;

			subscriptionsPool.Push(subscription);
		}

		private void TryRemoveFromBuffer(IPoolElement<IInvokableMultipleArgs> subscriptionElement)
		{
			if (!broadcastInProgress)
				return;
				
			for (int i = 0; i < currentSubscriptionsBufferCount; i++)
				if (currentSubscriptionsBuffer[i] == subscriptionElement.Value)
				{
					currentSubscriptionsBuffer[i] = null;

					return;
				}
		}
		
		#endregion

		#region IPublisherMultipleArgs
		
		public void Publish(object[] value)
		{
			ValidateBufferSize();

			currentSubscriptionsBufferCount = subscriptionsAsIndexable.Count;

			CopySubscriptionsToBuffer();

			InvokeSubscriptions(value);

			EmptyBuffer();
		}

		private void ValidateBufferSize()
		{
			if (currentSubscriptionsBuffer.Length < subscriptionsWithCapacity.Capacity)
				currentSubscriptionsBuffer = new IInvokableMultipleArgs[subscriptionsWithCapacity.Capacity];
		}

		private void CopySubscriptionsToBuffer()
		{
			for (int i = 0; i < currentSubscriptionsBufferCount; i++)
				currentSubscriptionsBuffer[i] = subscriptionsAsIndexable[i].Value;
		}

		private void InvokeSubscriptions(object[] value)
		{
			broadcastInProgress = true;

			for (int i = 0; i < currentSubscriptionsBufferCount; i++)
			{
				if (currentSubscriptionsBuffer[i] != null)
					currentSubscriptionsBuffer[i].Invoke(value);
			}

			broadcastInProgress = false;
		}

		private void EmptyBuffer()
		{
			for (int i = 0; i < currentSubscriptionsBufferCount; i++)
				currentSubscriptionsBuffer[i] = null;
		}
		
		#endregion
    }
}