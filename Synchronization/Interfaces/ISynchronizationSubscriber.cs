using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizationSubscriber
	{
		INonAllocSubscription SynchronizationSubscription { get; }
	}
}