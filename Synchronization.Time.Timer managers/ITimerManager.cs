using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;

using HereticalSolutions.Synchronization.Time.TimeUpdaters;

namespace HereticalSolutions.Synchronization.Time.TimerManagers
{
	public interface ITimerManager
	{
		string ID { get; }
		
		ITimeUpdater TimeUpdater { get; }

		bool TryAllocateTimer(
			out AllocatedTimerContext context);
		
		bool TryFreeTimer(
			AllocatedTimerContext context);
	}
}