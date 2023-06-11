using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    public class SetRuntimeTimerCallback<T> : IAllocationCallback<T>
    {
        public string ID { get; set; }

        public float DefaultDuration { get; set; }

        public SetRuntimeTimerCallback(
            string id = "Anonymous Timer",
            float defaultDuration = 0f)
        {
            ID = id;

            DefaultDuration = defaultDuration;
        }

        public void OnAllocated(IPoolElement<T> currentElement)
        {
            if (currentElement.Value == null)
                return;

            var metadata = (RuntimeTimerMetadata)currentElement.Metadata.Get<IContainsRuntimeTimer>();
            
            metadata.RuntimeTimer = TimeFactory.BuildRuntimeTimer(
                ID,
                DefaultDuration);

            metadata.RuntimeTimerAsTickable = (ITickable)metadata.RuntimeTimer;
            
            metadata.Subscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(metadata.RuntimeTimerAsTickable.Tick);
        }
    }
}