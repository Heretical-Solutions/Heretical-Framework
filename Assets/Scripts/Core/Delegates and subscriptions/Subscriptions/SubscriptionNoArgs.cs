using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionNoArgs
        : ISubscription,
          ISubscriptionState<IInvokableNoArgs>,
          ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>
    {
        private readonly IInvokableNoArgs invokable;
        
        private INonAllocSubscribableNoArgs publisher;

        private IPoolElement<IInvokableNoArgs> poolElement;
        
        public SubscriptionNoArgs(
            Action @delegate)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperNoArgs(@delegate);

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        public bool Active { get; private set;  }
        
        #endregion

        /*
        public void Subscribe(INonAllocSubscribableNoArgs publisher)
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

        #region ISubscriptionState

        public IInvokableNoArgs Invokable
        {
            get => invokable;
        }

        public IPoolElement<IInvokableNoArgs> PoolElement
        {
            get => poolElement;
        }
        
        #endregion

        #region ISubscriptionHandler
        
        public bool ValidateActivation(INonAllocSubscribableNoArgs publisher)
        {
            if (Active)
                throw new Exception("[SubscriptionNoArgs] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != null)
                throw new Exception("[SubscriptionNoArgs] SUBSCRIPTION ALREADY HAS A PUBLISHER");
			
            if (poolElement != null)
                throw new Exception("[SubscriptionNoArgs] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");
			
            if (invokable == null)
                throw new Exception("[SubscriptionNoArgs] INVALID DELEGATE");

            return true;
        }

        public void Activate(
            INonAllocSubscribableNoArgs publisher,
            IPoolElement<IInvokableNoArgs> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }

        public bool ValidateTermination(INonAllocSubscribableNoArgs publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionNoArgs] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != publisher)
                throw new Exception("[SubscriptionNoArgs] INVALID PUBLISHER");
			
            if (poolElement == null)
                throw new Exception("[SubscriptionNoArgs] INVALID POOL ELEMENT");

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