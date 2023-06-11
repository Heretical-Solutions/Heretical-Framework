using HereticalSolutions.Pools;

using UnityEngine;

namespace HereticalSolutions.GameEntities
{
    public class GameEntityPoolElementDescriptor
    {
        public bool HasPoolElement { get; private set; }

        public IPoolElement<GameObject> PoolElement { get; private set; }

        public INonAllocDecoratedPool<GameObject> Pool { get; private set; }
    }
}