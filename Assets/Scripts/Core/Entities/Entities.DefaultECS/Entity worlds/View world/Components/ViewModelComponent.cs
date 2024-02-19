using DefaultEcs;

namespace HereticalSolutions.Entities
{
    [Component("View world/References")]
    public struct ViewModelComponent
    {
        public Entity SourceEntity;
    }
}