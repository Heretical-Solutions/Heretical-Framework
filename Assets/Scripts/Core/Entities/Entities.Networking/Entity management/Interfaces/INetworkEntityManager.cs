using System;

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Interface for managing game entities.
	/// </summary>
	public interface INetworkEntityManager<TEntityID>
	{
		void SpawnEntityFromServer(
			TEntityID entityID,
			string prototypeID);
	}
}