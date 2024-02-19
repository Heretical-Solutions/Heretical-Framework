namespace HereticalSolutions.Entities
{
	public interface INetworkEventEntityBuilder<TEntity>
	{
		IEventEntityBuilder<TEntity> HostShouldBeNotified(TEntity eventEntity);

		IEventEntityBuilder<TEntity> PlayersShouldBeNotified(TEntity eventEntity);
	}
}