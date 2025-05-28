using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Timers.Factories
{
    public class TimerDecoratorAllocationCallbackFactory
    {
        private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

        private readonly ILoggerResolver loggerResolver;

        public TimerDecoratorAllocationCallbackFactory(
            NonAllocSubscriptionFactory nonAllocSubscriptionFactory,
            ILoggerResolver loggerResolver)
        {
            this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;

            this.loggerResolver = loggerResolver;
        }

        public SetPushToPoolOnTimeoutSubscriptionCallback<T> 
            BuildSetPushToPoolOnTimeoutSubscriptionCallback<T>()
        {
            var logger = loggerResolver?.
                GetLogger<SetPushToPoolOnTimeoutSubscriptionCallback<T>>();

            return new SetPushToPoolOnTimeoutSubscriptionCallback<T>(
                nonAllocSubscriptionFactory,
                logger);
        }
    }
}