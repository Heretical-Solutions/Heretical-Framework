using UnityEngine;

namespace HereticalSolutions.GameEntities
{
    public class GameEntityGameObjectDescriptor
    {
        public bool HasGameObject { get; private set; }

        public GameEntityAdapter Adapter { get; private set; }

        public GameObject GameObject
        {
            get
            {
                if (!HasGameObject)
                    return null;

                if (Adapter == null)
                    return null;

                return Adapter.gameObject;
            }
        }
    }
}