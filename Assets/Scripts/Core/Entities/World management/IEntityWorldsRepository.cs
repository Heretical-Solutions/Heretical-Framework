namespace HereticalSolutions.Entities
{
    public interface IEntityWorldsRepository<TWorld, TWorldController>
        : IReadOnlyEntityWorldsRepository<TWorld, TWorldController>
    {
        bool HasWorld(string worldID);

        bool HasWorld(TWorld world);


        void AddWorld(
            string worldID,
            TWorldController worldController);

        
        void RemoveWorld(string worldID);

        void RemoveWorld(TWorld world);
    }
}