using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates.Subscriptions
{
    public class SubscriptionMultipleArgs
        : ISubscription,
          ISubscriptionState<IInvokableMultipleArgs>,
          ISubscriptionHandler<INonAllocSubscribableMultipleArgs, IInvokableMultipleArgs>
    {
        private readonly IInvokableMultipleArgs invokable;
        
        private INonAllocSubscribableMultipleArgs publisher;

        private IPoolElement<IInvokableMultipleArgs> poolElement;
        
        public SubscriptionMultipleArgs(
            Action<object[]> @delegate)
        {
            invokable = DelegatesFactory.BuildDelegateWrapperMultipleArgs(@delegate);

            Active = false;

            publisher = null;

            poolElement = null;
        }

        #region ISubscription
        
        public bool Active { get; private set;  }

        #endregion
        
        /*
        public void Subscribe(INonAllocSubscribableMultipleArgs publisher)
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

        public IInvokableMultipleArgs Invokable
        {
            get => invokable;
        }

        public IPoolElement<IInvokableMultipleArgs> PoolElement
        {
            get => poolElement;
        }

        #endregion

        #region ISubscriptionHandler
        
        public bool ValidateActivation(INonAllocSubscribableMultipleArgs publisher)
        {
            if (Active)
                throw new Exception("[SubscriptionMultipleArgs] ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != null)
                throw new Exception("[SubscriptionMultipleArgs] SUBSCRIPTION ALREADY HAS A PUBLISHER");
			
            if (poolElement != null)
                throw new Exception("[SubscriptionMultipleArgs] SUBSCRIPTION ALREADY HAS A POOL ELEMENT");
			
            if (invokable == null)
                throw new Exception("[SubscriptionMultipleArgs] INVALID DELEGATE");

            return true;
        }
        
        public void Activate(
            INonAllocSubscribableMultipleArgs publisher,
            IPoolElement<IInvokableMultipleArgs> poolElement)
        {
            this.poolElement = poolElement;

            this.publisher = publisher;
            
            Active = true;
        }
        
        public bool ValidateTermination(INonAllocSubscribableMultipleArgs publisher)
        {
            if (!Active)
                throw new Exception("[SubscriptionMultipleArgs] ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE");
			
            if (this.publisher != publisher)
                throw new Exception("[SubscriptionMultipleArgs] INVALID PUBLISHER");
			
            if (poolElement == null)
                throw new Exception("[SubscriptionMultipleArgs] INVALID POOL ELEMENT");

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