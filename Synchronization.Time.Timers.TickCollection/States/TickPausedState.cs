namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public class TickPausedState
		: ITickTimerState
	{
		#region Progress

		public float GetProgressNormal(
			ITickTimerContext context)
		{
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
			//ENSURE THAT CALLING START() ON A PAUSED TIMER SHOULD IGNORE A CALL INSTEAD OF RESETTING THE TIMER
		}

		public void Pause(
			ITickTimerContext context)
		{
			//Why bother?
		}

		public void Resume(
			ITickTimerContext context)
		{
			context.SetState(ETimerState.STARTED);
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
			context.SetState(ETimerState.FINISHED);

			if (context.Repeat && context.FireRepeatCallbackOnFinish)
				context.OnFinishRepeated.Publish(context.Timer);

			context.OnFinish.Publish(context.Timer);
		}

		public void Tick(
			ITickTimerContext context)
		{
			//Why bother?
		}

		#endregion
	}
}