using DefaultEcs;

namespace HereticalSolutions.Entities
{
    [Component("View world/View models")]
    public struct ViewModelComponent
    {
        public Entity SourceEntity;
    }
}