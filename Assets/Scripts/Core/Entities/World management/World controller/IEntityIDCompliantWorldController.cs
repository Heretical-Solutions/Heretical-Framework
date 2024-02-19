using System;

namespace HereticalSolutions.Entities
{
	public interface IEntityIDCompliantWorldController<TEntityID, TEntity>
	{
		bool TrySpawnEntityWithIDFromPrototype(
			string prototypeID,
			TEntityID entityID,
			out TEntity entity);

		bool TrySpawnAndResolveEntityWithIDFromPrototype(
			string prototypeID,
			TEntityID entityID,
			object source,
			out TEntity entity);
	}
}