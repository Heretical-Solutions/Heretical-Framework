namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public class TickFinishedState
		: ITickTimerState
	{
		#region Progress

		public float GetProgressNormal(
			ITickTimerContext context)
		{
			//THIS ONE IS AS EXPECTED. IF THE TIMER WAS FINISHED PREMATURELY BY A FINISH() CALL RATHER THAN TIMER ACTUALLY RUNNING OUT WE MIGHT BE CURIOUS HOW MUCH OF A PROGRESS WAS MADE SO FAR
			if (context.Accumulate)
				return 0f;

			if ((context.CurrentDuration - MathHelpers.EPSILON) < 0f)
				return 0f;

			return ((float)context.CurrentTimeElapsed / (float)context.CurrentDuration)
				.Clamp(0f, 1f);
		}

		#endregion

		#region Countdown and Time elapsed

		public uint GetTimeElapsed(
			ITickTimerContext context)
		{
			return context.CurrentTimeElapsed;
		}

		public uint GetCountdown(
			ITickTimerContext context)
		{
			return context.CurrentDuration - context.CurrentTimeElapsed;
		}

		#endregion

		#region Controls

		public void Reset(
			ITickTimerContext context)
		{
			context.CurrentTimeElapsed = 0;

			context.CurrentDuration = context.DefaultDuration;

			context.SetState(ETimerState.INACTIVE);
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

			context.SetState(ETimerState.INACTIVE);
		}

		public void Finish(
			ITickTimerContext context)
		{
			//Why bother?
		}

		public void Tick(
			ITickTimerContext context)
		{
			//Why bother?
		}

		#endregion
	}
}