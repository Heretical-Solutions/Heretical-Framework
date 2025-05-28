namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
	public interface IContainsTimeSpanTimer
	{
		ITimeSpanTimer Timer { get; set; }
	}
}