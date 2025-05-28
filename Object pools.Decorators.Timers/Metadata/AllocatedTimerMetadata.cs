using System;

using HereticalSolutions.Delegates.NonAlloc;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Synchronization.Time.TimerManagers;

namespace HereticalSolutions.ObjectPools.Decorators.Timers
{
    public class AllocatedTimerMetadata
        : IContainsAllocatedTimer,
          ICleanuppable,
          IDisposable
    {
        public AllocatedTimerMetadata()
        {
            TimerContext = null;

            PushToPoolOnTimeoutSubscription = null;
        }

        #region IContainsRuntimeTimer

        public AllocatedTimerContext TimerContext { get; set; }

        public INonAllocSubscription PushToPoolOnTimeoutSubscription { get; set; }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (TimerContext is ICleanuppable)
                (TimerContext as ICleanuppable).Cleanup();

            if (PushToPoolOnTimeoutSubscription is ICleanuppable)
                (PushToPoolOnTimeoutSubscription as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (TimerContext is IDisposable)
                (TimerContext as IDisposable).Dispose();

            if (PushToPoolOnTimeoutSubscription is IDisposable)
                (PushToPoolOnTimeoutSubscription as IDisposable).Dispose();
        }

        #endregion
    }
}