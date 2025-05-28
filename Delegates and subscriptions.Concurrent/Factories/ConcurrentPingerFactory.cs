using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.Delegates.Concurrent.Factories
{
	public class ConcurrentPingerFactory
	{
		#region Concurrent pinger

		public ConcurrentPinger BuildConcurrentPinger(
			IPool<PingerInvocationContext> contextPool)
		{
			return new ConcurrentPinger(
				contextPool,
				new object());
		}

		#endregion
	}
}