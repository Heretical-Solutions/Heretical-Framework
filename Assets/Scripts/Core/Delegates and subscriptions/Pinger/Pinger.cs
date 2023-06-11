using System;

namespace HereticalSolutions.Delegates.Pinging
{
	public class Pinger
		: IPublisherNoArgs,
		  ISubscribableNoArgs
	{
		private Action multicastDelegate;

		#region IPublisherNoArgs
		
		public void Publish()
		{
			multicastDelegate?.Invoke();
		}

		#endregion
		
		#region ISubscribableNoArgs
		
		public void Subscribe(Action @delegate)
		{
			multicastDelegate += @delegate;
		}
        
		public void Unsubscribe(Action @delegate)
		{
			multicastDelegate -= @delegate;
		}
        
		#endregion
	}
}