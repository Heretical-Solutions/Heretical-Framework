namespace HereticalSolutions.Entities
{
	public interface IContainsEntityWorlds<TWorld, TSystem, TEntity>
	{
		IReadOnlyEntityWorldsRepository<TWorld, TSystem, TEntity> EntityWorldsRepository { get; }
	}
}