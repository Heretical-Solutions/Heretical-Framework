namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public interface ITickTimerContext
		: ITimerContext<ITickTimer, ITickTimerState, TickTimerDTO>
	{
		uint CurrentTimeElapsed { get; set; }

		uint CurrentDuration { get; set; }

		uint DefaultDuration { get; }
	}
}