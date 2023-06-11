using HereticalSolutions.Delegates;

namespace HereticalSolutions.Time
{
    public class SynchronizationContext
        : ISynchronizable,
          ISynchronizationProvider
    {
        public SynchronizationContextDescriptor Descriptor { get; private set; }

        private readonly IPublisherSingleArgGeneric<float> broadcasterAsPublisher;

        private readonly INonAllocSubscribableSingleArgGeneric<float> broadcasterAsSubscribable;

        public SynchronizationContext(
            SynchronizationContextDescriptor descriptor,
            IPublisherSingleArgGeneric<float> broadcasterAsPublisher,
            INonAllocSubscribableSingleArgGeneric<float> broadcasterAsSubscribable)
        {
            Descriptor = descriptor;

            this.broadcasterAsPublisher = broadcasterAsPublisher;

            this.broadcasterAsSubscribable = broadcasterAsSubscribable;
        }

        #region ISynchronizable

        public void Synchronize(float delta)
        {
            if (Descriptor.CanBeToggled && !Descriptor.Active)
                return;
            
            if (Descriptor.CanScale)
                broadcasterAsPublisher.Publish(delta * Descriptor.Scale);
            else
                broadcasterAsPublisher.Publish(delta);
        }

        #endregion

        #region ISynchronizationProvider

        public void Subscribe(ISubscription subscription)
        {
            broadcasterAsSubscribable.Subscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<float>, IInvokableSingleArgGeneric<float>>)subscription);
        }

        //TODO: Add UnsubscribeAll
        public void Unsubscribe(ISubscription subscription)
        {
            broadcasterAsSubscribable.Unsubscribe((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<float>, IInvokableSingleArgGeneric<float>>)subscription);
        }

        #endregion
    }
}