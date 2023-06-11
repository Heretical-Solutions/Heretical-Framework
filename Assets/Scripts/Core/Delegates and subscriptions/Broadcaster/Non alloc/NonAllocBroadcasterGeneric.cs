using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Broadcasting
{
	public class NonAllocBroadcasterGeneric<TValue>
		: IPublisherSingleArgGeneric<TValue>,
		  IPublisherSingleArg,
		  INonAllocSubscribableSingleArgGeneric<TValue>,
		  INonAllocSubscribableSingleArg
	{
		#region Subscriptions
		
		private readonly INonAllocDecoratedPool<IInvokableSingleArgGeneric<TValue>> subscriptionsPool;

		private readonly IIndexable<IPoolElement<IInvokableSingleArgGeneric<TValue>>> subscriptionsAsIndexable;

		private readonly IFixedSizeCollection<IPoolElement<IInvokableSingleArgGeneric<TValue>>> subscriptionsWithCapacity;

		#endregion
		
		#region Buffer

		private IInvokableSingleArgGeneric<TValue>[] currentSubscriptionsBuffer;

		private int currentSubscriptionsBufferCount = -1;

		#endregion
		
		private bool broadcastInProgress = false;

		public NonAllocBroadcasterGeneric(
			INonAllocDecoratedPool<IInvokableSingleArgGeneric<TValue>> subscriptionsPool,
			INonAllocPool<IInvokableSingleArgGeneric<TValue>> subscriptionsContents)
		{
			this.subscriptionsPool = subscriptionsPool;

			subscriptionsAsIndexable = (IIndexable<IPoolElement<IInvokableSingleArgGeneric<TValue>>>)subscriptionsContents;

			subscriptionsWithCapacity =
				(IFixedSizeCollection<IPoolElement<IInvokableSingleArgGeneric<TValue>>>)subscriptionsContents;

			currentSubscriptionsBuffer = new IInvokableSingleArgGeneric<TValue>[subscriptionsWithCapacity.Capacity];
		}

		#region INonAllocSubscribableSingleArgGeneric
		
		public void Subscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
		{
			if (!subscription.ValidateActivation(this))
				return;
			
			var subscriptionElement = subscriptionsPool.Pop(null);

			subscriptionElement.Value = ((ISubscriptionState<IInvokableSingleArgGeneric<TValue>>)subscription).Invokable;

			subscription.Activate(this, subscriptionElement);
		}

		public void Unsubscribe(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>> subscription)
		{
			if (!subscription.ValidateTermination(this))
				return;

			var poolElement = ((ISubscriptionState<IInvokableSingleArgGeneric<TValue>>)subscription).PoolElement;
			
			TryRemoveFromBuffer(poolElement);
			
			poolElement.Value = null;

			subscriptionsPool.Push(poolElement);
			
			subscription.Terminate();
		}

		private void TryRemoveFromBuffer(IPoolElement<IInvokableSingleArgGeneric<TValue>> subscriptionElement)
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

		#region INonAllocSubscribableSingleArg

		public void Subscribe<TArgument>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TArgument>, IInvokableSingleArgGeneric<TArgument>> subscription)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
            
            Subscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>)subscription);
		}

		public void Subscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

			Subscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>)subscription);
		}

		public void Unsubscribe<TArgument>(ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TArgument>, IInvokableSingleArgGeneric<TArgument>> subscription)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
            
			Unsubscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>)subscription);
		}

		public void Unsubscribe(Type valueType, ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg> subscription)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

			Unsubscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>)subscription);
		}

		#endregion
		
		#region IPublisherSingleArgGeneric
		
		public void Publish(TValue value)
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
				currentSubscriptionsBuffer = new IInvokableSingleArgGeneric<TValue>[subscriptionsWithCapacity.Capacity];
		}

		private void CopySubscriptionsToBuffer()
		{
			for (int i = 0; i < currentSubscriptionsBufferCount; i++)
				currentSubscriptionsBuffer[i] = subscriptionsAsIndexable[i].Value;
		}

		private void InvokeSubscriptions(TValue value)
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
		
		#region IPublisherSingleArg
		
		public void Publish<TArgument>(TArgument value)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
			
			//DIRTY HACKS DO NOT REPEAT
			object valueObject = (object)value;
			
			Publish((TValue)valueObject); //It doesn't want to convert TArgument into TValue. Bastard
		}

		public void Publish(Type valueType, object value)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[NonAllocBroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");
			
			Publish((TValue)value);
		}
		
		#endregion
	}
}