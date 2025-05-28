namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public interface ITickTimer
		: ITimer
	{
		ITickTimerContext Context { get; }

		#region Countdown and Time elapsed

		uint TicksElapsed { get; }

		uint Countdown { get; }

		#endregion

		#region Duration

		uint CurrentDuration { get; }

		uint DefaultDuration { get; }

		#endregion

		#region Controls

		void Reset(
			uint duration);

		void Start(
			uint duration);

		void Resume(
			uint duration);

		#endregion
	}
}