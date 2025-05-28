using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    public interface ITimeSpanTimerContext
        : ITimerContext<ITimeSpanTimer, ITimeSpanTimerState, TimeSpanTimerDTO>
    {
        DateTime StartTime { get; set; }
        
        DateTime EstimatedFinishTime { get; set; }

        TimeSpan SavedProgress { get; set; }
        
        TimeSpan CurrentDurationSpan { get; set; }
        
        TimeSpan DefaultDurationSpan { get; set; }
    }
}