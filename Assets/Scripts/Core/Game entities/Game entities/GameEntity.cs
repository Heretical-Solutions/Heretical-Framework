using System;

namespace HereticalSolutions.GameEntities
{
    public class GameEntity
    {
        public Guid GUID { get; private set; }

        public string Name { get; private set; }

        //public EGameEntityAuthoringMethods AuthoringMethod { get; private set; }
    }
}