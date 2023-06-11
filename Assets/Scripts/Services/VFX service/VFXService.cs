using HereticalSolutions.Messaging;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

using UnityEngine;

namespace HereticalSolutions.Services
{
    public class VFXService
    {
        private IRepository<string, INonAllocDecoratedPool<GameObject>> vfxPoolsRepository;

        private MessageBus vfxBus;
        
        
    }
}