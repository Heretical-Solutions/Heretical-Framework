using DefaultEcs;

namespace HereticalSolutions.GameEntities
{
    public class GameEntityECSDescriptor
    {
        public bool HasECSEntity { get; private set; }

        public Entity ECSEntity { get; private set; }

        public World ECSWorld { get; private set; }
    }
}