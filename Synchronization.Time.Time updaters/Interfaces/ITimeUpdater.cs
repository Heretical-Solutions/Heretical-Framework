using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;

namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
    public interface ITimeUpdater
        : ISynchronizationWithDeltaProvider<float>,
          ISynchronizableWithDelta<float>,
          ISynchronizationSubscriber
    {
        TimeUpdaterDescriptor Descriptor { get; }

        IFloatTimer Accumulator { get; }
    }
}