namespace HereticalSolutions.Entities
{
	public interface INetworkEventEntityBuilder<TEntity, TEntityID>
	{
		IEventEntityBuilder<TEntity, TEntityID> HostShouldBeNotified(TEntity eventEntity);

		IEventEntityBuilder<TEntity, TEntityID> PlayersShouldBeNotified(TEntity eventEntity);
	}
}