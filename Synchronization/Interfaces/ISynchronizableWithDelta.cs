namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizableWithDelta<TDelta>
	{
		void Synchronize(
			TDelta delta);
	}
}