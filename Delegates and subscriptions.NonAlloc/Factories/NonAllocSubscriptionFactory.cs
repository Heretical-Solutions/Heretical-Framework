using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Factories
{
    public class NonAllocSubscriptionFactory
    {
        private readonly DelegateWrapperFactory delegateWrapperFactory;

        private readonly ILoggerResolver loggerResolver;

        public NonAllocSubscriptionFactory(
            DelegateWrapperFactory delegateWrapperFactory,
            ILoggerResolver loggerResolver)
        {
            this.delegateWrapperFactory = delegateWrapperFactory;

            this.loggerResolver = loggerResolver;
        }

        #region Subscriptions

        public SubscriptionNoArgs
            BuildSubscriptionNoArgs(
                Action @delegate)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionNoArgs>();

            IInvokableNoArgs invokable = delegateWrapperFactory
                .BuildDelegateWrapperNoArgs(
                    @delegate);

            return new SubscriptionNoArgs(
                invokable,
                logger);
        }
        
        public SubscriptionSingleArgGeneric<TValue>
            BuildSubscriptionSingleArgGeneric<TValue>(
                Action<TValue> @delegate)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionSingleArgGeneric<TValue>>();

            IInvokableSingleArgGeneric<TValue> invokable = delegateWrapperFactory.
                BuildDelegateWrapperSingleArgGeneric(
                    @delegate);

            return new SubscriptionSingleArgGeneric<TValue>(
                invokable,
                logger);
        }
        
        public SubscriptionMultipleArgs
            BuildSubscriptionMultipleArgs(
                Action<object[]> @delegate)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionMultipleArgs>();

            IInvokableMultipleArgs invokable = delegateWrapperFactory.
                BuildDelegateWrapperMultipleArgs(
                    @delegate);

            return new SubscriptionMultipleArgs(
                invokable,
                logger);
        }

        #endregion
    }
}