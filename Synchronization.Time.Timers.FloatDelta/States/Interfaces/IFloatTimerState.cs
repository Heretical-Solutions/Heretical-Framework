namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
	public interface IFloatTimerState
		: ITimerState<IFloatTimerContext, float>
	{
		void Tick(
			IFloatTimerContext context,
			float delta);
	}
}