using HereticalSolutions.Synchronization.Time.Timers.TickCollection;

namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
	public interface ITickUpdater
		: ISynchronizationProvider,
		  ISynchronizable,
		  ISynchronizationSubscriber
	{
		TickUpdaterDescriptor Descriptor { get; }

		ITickTimer Accumulator { get; }
	}
}