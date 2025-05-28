using HereticalSolutions.Delegates.NonAlloc;

using HereticalSolutions.Synchronization.Time.TimerManagers;

namespace HereticalSolutions.ObjectPools.Decorators.Timers
{
	public interface IContainsAllocatedTimer
	{
		AllocatedTimerContext TimerContext { get; }

		INonAllocSubscription PushToPoolOnTimeoutSubscription { get; }
	}
}