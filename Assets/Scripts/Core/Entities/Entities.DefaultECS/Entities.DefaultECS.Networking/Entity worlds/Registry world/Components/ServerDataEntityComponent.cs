using DefaultEcs;

namespace HereticalSolutions.Entities
{
    [Component("Registry world")]
    [IdentityComponent]
    public struct ServerDataEntityComponent
    {
        public Entity ServerDataEntity;

        public string PrototypeID;
    }
}