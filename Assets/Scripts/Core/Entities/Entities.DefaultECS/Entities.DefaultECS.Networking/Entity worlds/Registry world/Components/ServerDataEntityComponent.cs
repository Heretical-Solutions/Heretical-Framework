using DefaultEcs;

namespace HereticalSolutions.Entities
{
    [Component("Server world")]
    [WorldIdentityComponent]
    public struct ServerDataEntityComponent
    {
        public Entity ServerDataEntity;

        public string PrototypeID;
    }
}