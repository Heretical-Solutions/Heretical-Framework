using System;

using HereticalSolutions.Delegates.Subscriptions;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Subscriptions

        public static SubscriptionNoArgs BuildSubscriptionNoArgs(Action @delegate)
        {
            return new SubscriptionNoArgs(@delegate);
        }
        
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(Action<TValue> @delegate)
        {
            return new SubscriptionSingleArgGeneric<TValue>(@delegate);
        }
        
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(Action<object[]> @delegate)
        {
            return new SubscriptionMultipleArgs(@delegate);
        }

        #endregion
    }
}