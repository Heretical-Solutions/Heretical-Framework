using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Synchronization.Time.TimerManagers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Timers.Factories
{
    public class TimerDecoratorPoolFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public TimerDecoratorPoolFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        public TimerDecoratorManagedPool<T> BuildTimerDecoratorManagedPool<T>(
            IManagedPool<T> innerPool,
            ITimerManager timerManager,
            float duration = 0f)
        {
            ILogger logger =
                loggerResolver?.GetLogger<TimerDecoratorManagedPool<T>>();

            return new TimerDecoratorManagedPool<T>(
                innerPool,
                timerManager,
                duration,
                logger);
        }
    }
}