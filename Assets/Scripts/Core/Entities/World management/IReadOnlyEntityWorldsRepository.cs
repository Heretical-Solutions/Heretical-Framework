namespace HereticalSolutions.Entities
{
	public interface IReadOnlyEntityWorldsRepository<TWorld, TWorldController>
	{
		TWorld GetWorld(string worldID);

		TWorldController GetWorldController(string worldID);

		TWorldController GetWorldController(TWorld world);
	}
}