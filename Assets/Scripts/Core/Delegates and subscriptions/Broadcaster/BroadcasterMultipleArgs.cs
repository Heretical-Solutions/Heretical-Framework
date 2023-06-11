using System;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class BroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          ISubscribableMultipleArgs
    {
        private readonly BroadcasterGeneric<object[]> innerBroadcaster;

        public BroadcasterMultipleArgs(BroadcasterGeneric<object[]> innerBroadcaster)
        {
            this.innerBroadcaster = innerBroadcaster;
        }

        #region IPublisherMultipleArgs

        public void Publish(object[] values)
        {
            innerBroadcaster.Publish(values);
        }

        #endregion

        #region ISubscribableMultipleArgs
		
        public void Subscribe(Action<object[]> @delegate)
        {
            
            innerBroadcaster.Subscribe(@delegate);
        }

        public void Unsubscribe(Action<object[]> @delegate)
        {
            innerBroadcaster.Unsubscribe(@delegate);
        }

        #endregion
    }
}