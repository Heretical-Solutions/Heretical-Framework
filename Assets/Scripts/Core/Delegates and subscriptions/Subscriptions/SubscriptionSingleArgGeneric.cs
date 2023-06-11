using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionSingleArgGeneric<TValue>
        : ISubscription,
          ISubscriptionState<IInvokableSingleArgGeneric<TValue>>,
          ISubscriptionState<IInvokableSingleArg>,
          ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TValue>, IInvokableSingleArgGeneric<TValue>>,
          ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>
    {
        private readonly IInvokableSingleArgGeneric<TValue> invokable;
        
        private object publisher;

        private object poolElement;
        
        public SubscriptionSingleArgGeneric(
            Action<TValue> @delegate)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperSingleArgGeneric(@delegate);

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        public bool Active { get; private set;  }

        #endregion

        /*
        public void Subscribe(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (Active)
                return;
            
            publisher.Subscribe(this);
        }

        public void Unsubscribe()
        {
            if (!Active)
                return;

            publisher.Unsubscribe(this);
        }
        */

        #region ISubscriptionState (Generic)

        IInvokableSingleArgGeneric<TValue> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.Invokable
        {
            get
            {
                return (IInvokableSingleArgGeneric<TValue>)invokable;
            }
        }

        IPoolElement<IInvokableSingleArg> ISubscriptionState<IInvokableSingleArg>.PoolElement
        {
            get
            {
                return (IPoolElement<IInvokableSingleArg>)poolElement;
            }
        }
        
        #endregion
        
        #region ISubscriptionState

        IInvokableSingleArg ISubscriptionState<IInvokableSingleArg>.Invokable
        {
            get => (IInvokableSingleArg)invokable;
        }

        IPoolElement<IInvokableSingleArgGeneric<TValue>> ISubscriptionState<IInvokableSingleArgGeneric<TValue>>.PoolElement
        {
            get =>  (IPoolElement<IInvokableSingleArgGeneric<TValue>>)poolElement;
        }
        
        #endregion

        #region ISubscriptionHandler (Generic)

        public bool ValidateActivation(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A PUBLISHER");
			
            if (poolElement != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");
			
            if (invokable == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID DELEGATE");

            return true;
        }
        
        public void Activate(
            INonAllocSubscribableSingleArgGeneric<TValue> publisher,
            IPoolElement<IInvokableSingleArgGeneric<TValue>> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }
        
        public bool ValidateTermination(INonAllocSubscribableSingleArgGeneric<TValue> publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY INACTIVE");
			
            if (this.publisher != publisher)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID PUBLISHER");
			
            if (poolElement == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID POOL ELEMENT");

            return true;
        }
        
        #endregion

        #region ISubscriptionHandler
        
        public bool ValidateActivation(INonAllocSubscribableSingleArg publisher)
        {
            if (Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A PUBLISHER");
			
            if (poolElement != null)
                throw new Exception("[SubscriptionSingleArgGeneric] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");
			
            if (invokable == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID DELEGATE");

            return true;
        }

        public void Activate(
            INonAllocSubscribableSingleArg publisher,
            IPoolElement<IInvokableSingleArg> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }

        public bool ValidateTermination(INonAllocSubscribableSingleArg publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionSingleArgGeneric] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != publisher)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID PUBLISHER");
			
            if (poolElement == null)
                throw new Exception("[SubscriptionSingleArgGeneric] INVALID POOL ELEMENT");

            return true;
        }

        public void Terminate()
        {
            poolElement = null;
            
            publisher = null;
            
            Active = false;
        }

        #endregion
    }
}