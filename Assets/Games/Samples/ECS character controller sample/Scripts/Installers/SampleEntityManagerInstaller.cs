using System;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Entities;
using HereticalSolutions.Entities.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample.Installers
{
    public class SampleEntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        public override void InstallBindings()
        {
            Func<Guid> allocateIDDelegate = () => 
            {
                return IDAllocationsFactory.BuildGUID();
            };

            Func<GUIDComponent, Guid> getEntityIDFromIDComponentDelegate = (GUIDComponent) =>
            {
                return GUIDComponent.GUID;
            };

            Func<Guid, GUIDComponent> createIDComponentDelegate = (guid) =>
            {
                return new GUIDComponent
                {
                    GUID = guid
                };
            };

            var entityManager = DefaultECSEntitiesFactory.BuildDefaultECSSimpleEntityManager<Guid, GUIDComponent>(
                allocateIDDelegate,
                getEntityIDFromIDComponentDelegate,
                createIDComponentDelegate,
                loggerResolver);

            Container
                .Bind<IEntityManager<World, Guid, Entity>>()
                .FromInstance(entityManager)
                .AsCached();
        }
    }
}