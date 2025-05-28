namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    public interface IFloatTimerContext
        : ITimerContext<IFloatTimer, IFloatTimerState, FloatTimerDTO>
    {
        float CurrentTimeElapsed { get; set; }

        float CurrentDuration { get; set; }

        float DefaultDuration { get; }
    }
}