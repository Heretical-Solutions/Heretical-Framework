namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public interface ITickTimerState
		: ITimerState<ITickTimerContext, uint>
	{
		void Tick(
			ITickTimerContext context);
	}
}