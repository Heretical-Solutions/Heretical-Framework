namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizationWithDeltaProvider<T>
	{
		void Subscribe(
			ISynchronizableWithDelta<T> synchronizable);

		void Unsubscribe(
			ISynchronizableWithDelta<T> synchronizable);

		void UnsubscribeAll();
	}
}