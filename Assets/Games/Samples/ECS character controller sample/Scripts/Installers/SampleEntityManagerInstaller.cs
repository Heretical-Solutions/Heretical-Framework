using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;

using Zenject;

using HereticalSolutions.Sample.ECSCharacterControllerSample.Factories;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample.Installers
{
    public class SampleEntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        public override void InstallBindings()
        {
            var entityManager = SampleEntityFactory.BuildSampleEntityManager(
                loggerResolver);

            Container
                .Bind<SampleEntityManager>()
                .FromInstance(entityManager)
                .AsCached();
        }
    }
}