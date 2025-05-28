namespace HereticalSolutions.Synchronization.Time.Timers
{
	public interface ITimerDependencyProvideable<TTimer>
		where TTimer : ITimer
	{
		TTimer Timer { set; }
	}
}