using System;

namespace HereticalSolutions.GameEntities
{
    [Serializable]
    public struct EntityDescriptor<TEntityID>
    {
        public TEntityID ID;

        public string PrototypeID;
    }
}