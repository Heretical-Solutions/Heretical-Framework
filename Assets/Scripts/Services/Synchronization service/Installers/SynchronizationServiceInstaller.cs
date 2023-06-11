using HereticalSolutions.Services.Factories;
using HereticalSolutions.Time.Factories;
using UnityEngine;

using Zenject;

namespace HereticalSolutions.Services.DI
{
    public class SynchronizationServiceInstaller : MonoInstaller
    {
        [SerializeField]
        private SynchronizationService synchronizationService;
        
        public override void InstallBindings()
        {
            var timeManager = TimeFactory.BuildTimeManager();
            
            ServicesFactory.InitializeSynchronizationService(
                synchronizationService,
                timeManager);
            
            Container
                .BindInterfacesAndSelfTo<SynchronizationService>()
                .FromInstance(synchronizationService)
                .AsCached();
        }
    }
}