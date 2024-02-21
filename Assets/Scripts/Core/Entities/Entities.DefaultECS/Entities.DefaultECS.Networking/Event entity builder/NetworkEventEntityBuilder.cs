using DefaultEcs;

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Represents a class for building event entities.
	/// </summary>
	public class NetworkEventEntityBuilder<TEntityID>
		: DefaultECSEventEntityBuilder<TEntityID>,
		  INetworkEventEntityBuilder<Entity, TEntityID>
	{
		public NetworkEventEntityBuilder(
			World eventWorld)
			: base (eventWorld)
		{
		}

        public IEventEntityBuilder<Entity, TEntityID> HostShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyHostComponent>(new NotifyHostComponent());
            
            return this;
        }

        public IEventEntityBuilder<Entity, TEntityID> PlayersShouldBeNotified(Entity eventEntity)
        {
            eventEntity.Set<NotifyPlayersComponent>(new NotifyPlayersComponent());
            
            return this;
        }
	}
}