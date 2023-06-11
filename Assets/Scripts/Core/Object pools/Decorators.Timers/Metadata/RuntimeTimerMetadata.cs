using HereticalSolutions.Delegates;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    public class RuntimeTimerMetadata : IContainsRuntimeTimer
    {
        public IRuntimeTimer RuntimeTimer { get; set; }
        
        public ITickable RuntimeTimerAsTickable { get; set; }

        public ISubscription Subscription { get; set; }

        public RuntimeTimerMetadata()
        {
            RuntimeTimer = null;

            RuntimeTimerAsTickable = null;

            Subscription = null;
        }
    }
}