using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc
{
    public class SubscriptionNoArgs
        : INonAllocSubscription,
          INonAllocSubscriptionContext<IInvokableNoArgs>,
          ICleanuppable,
          IDisposable
    {
        private readonly IInvokableNoArgs @delegate;

        private readonly ILogger logger;
        
        private INonAllocSubscribable publisher;

        public SubscriptionNoArgs(
            IInvokableNoArgs @delegate,
            ILogger logger)
        {
            this.@delegate = @delegate;

            this.logger = logger;

            Active = false;

            publisher = null;
        }

        #region INonAllocSubscription
        
        public bool Active { get; private set;  }

        public bool Subscribe(
            INonAllocSubscribable publisher)
        {
            if (Active)
                return false;

            return publisher.Subscribe(this);
        }

        public bool Unsubscribe()
        {
            if (!Active)
                return false;

            return publisher.Unsubscribe(this);
        }


        #endregion

        #region INonAllocSubscriptionContext

        public IInvokableNoArgs Delegate
        {
            get => @delegate;
        }

        public bool ValidateActivation(
            INonAllocSubscribable publisher)
        {
            if (Active)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO ACTIVATE A SUBSCRIPTION THAT IS ALREADY ACTIVE: {this.GetHashCode()}");

                return false;
            }

            if (this.publisher != null)
            {
                logger?.LogError(
                    GetType(),
                    $"SUBSCRIPTION ALREADY HAS A PUBLISHER: {this.GetHashCode()}");

                return false;
            }

            if (@delegate == null)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID DELEGATE: {this.GetHashCode()}");

                return false;
            }

            if (publisher is not IPublisherNoArgs)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PUBLISHER: EXPECTED {nameof(IPublisherNoArgs)} : {this.GetHashCode()}");

                return false;
            }

            return true;
        }

        public void Activate(
            INonAllocSubscribable publisher)
        {
            this.publisher = publisher;

            Active = true;

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION ACTIVATED: {this.GetHashCode()}");
        }

        public bool ValidateTermination(
            INonAllocSubscribable publisher)
        {
            if (!Active)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO TERMINATE A SUBSCRIPTION THAT IS ALREADY ACTIVE: {this.GetHashCode()}");

                return false;
            }

            if (this.publisher != publisher)
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PUBLISHER: {this.GetHashCode()}");

                return false;
            }

            return true;
        }

        public void Terminate()
        {
            publisher = null;

            Active = false;

            logger?.Log(
                GetType(),
                $"SUBSCRIPTION TERMINATED: {this.GetHashCode()}");
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (Active
                && publisher != null
                && publisher.Unsubscribe(this))
            {
            }
            else
                Terminate();

            if (@delegate is ICleanuppable)
                (@delegate as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (Active
                && publisher != null
                && publisher.Unsubscribe(this))
            {
            }
            else
                Terminate();

            if (@delegate is IDisposable)
                (@delegate as IDisposable).Dispose();
        }

        #endregion
    }
}