using HereticalSolutions.Delegates;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    public interface IContainsRuntimeTimer
    {
        IRuntimeTimer RuntimeTimer { get; }

        ITickable RuntimeTimerAsTickable { get; }

        ISubscription Subscription { get; }
    }
}