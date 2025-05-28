namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public class TickInactiveState
		: ITickTimerState
	{
		#region Progress

		public float GetProgressNormal(
			ITickTimerContext context)
		{
			return 0f;
		}

		#endregion

		#region Countdown and Time elapsed

		public uint GetTimeElapsed(
			ITickTimerContext context)
		{
			return 0;
		}

		public uint GetCountdown(
			ITickTimerContext context)
		{
			return 0;
		}

		#endregion

		#region Controls

		public void Reset(
			ITickTimerContext context)
		{
			context.CurrentTimeElapsed = 0;

			context.CurrentDuration = context.DefaultDuration;
		}

		public void Start(
			ITickTimerContext context)
		{
			context.CurrentTimeElapsed = 0;

			context.SetState(ETimerState.STARTED);

			context.OnStart.Publish(context.Timer);
		}

		public void Pause(
			ITickTimerContext context)
		{
			//Why bother?
		}

		public void Resume(
			ITickTimerContext context)
		{
			//Why bother?
		}

		public void Abort(
			ITickTimerContext context)
		{
			context.CurrentTimeElapsed = 0;
		}

		public void Finish(
			ITickTimerContext context)
		{
			//ENSURE WHETHER CALLING FINISH() ON INACTIVE TIMER SHOULD NOT BE CALLING A CALLBACK
		}

		public void Tick(
			ITickTimerContext context)
		{
			//Why bother?
		}

		#endregion
	}
}