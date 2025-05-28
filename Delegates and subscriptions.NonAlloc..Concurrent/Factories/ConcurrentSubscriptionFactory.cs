using System;

using HereticalSolutions.Delegates.NonAlloc.Factories;

namespace HereticalSolutions.Delegates.NonAlloc.Concurrent.Factories
{
    public class ConcurrentSubscriptionFactory
    {
        private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

        public ConcurrentSubscriptionFactory(
            NonAllocSubscriptionFactory nonAllocSubscriptionFactory)
        {
            this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;
        }

        #region Concurrent subscriptions

        public ConcurrentSubscriptionNoArgs
            BuildConcurrentSubscriptionNoArgs(
                Action @delegate)
        {
            return new ConcurrentSubscriptionNoArgs(
                nonAllocSubscriptionFactory.
                    BuildSubscriptionNoArgs(
                        @delegate),
                new object());
        }

        public ConcurrentSubscriptionSingleArgGeneric<TValue>
            BuildConcurrentSubscriptionSingleArgGeneric<TValue>(
                Action<TValue> @delegate)
        {
            return new ConcurrentSubscriptionSingleArgGeneric<TValue>(
                nonAllocSubscriptionFactory
                    .BuildSubscriptionSingleArgGeneric<TValue>(
                        @delegate),
                new object());
        }

        public ConcurrentSubscriptionMultipleArgs
            BuildConcurrentSubscriptionMultipleArgs(
                Action<object[]> @delegate)
        {
            return new ConcurrentSubscriptionMultipleArgs(
                nonAllocSubscriptionFactory
                    .BuildSubscriptionMultipleArgs(
                        @delegate),
                new object());
        }

        #endregion
    }
}