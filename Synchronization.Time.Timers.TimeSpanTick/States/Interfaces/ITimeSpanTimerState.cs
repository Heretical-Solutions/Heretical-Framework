using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
	public interface ITimeSpanTimerState
		: ITimerState<ITimeSpanTimerContext, TimeSpan>
	{
		void Tick(
			ITimeSpanTimerContext context);
	}
}