/*
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Hierarchy;
using HereticalSolutions.Hierarchy.Factories;

using HereticalSolutions.LifetimeManagement.Factories;

using HereticalSolutions.ObjectPools;
using HereticalSolutions.ObjectPools.Factories;

using HereticalSolutions.LifetimeManagement.NonAlloc.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.LifetimeManagement.NonAlloc.Unity.Zenject
{
    public class ProjectLifetimeInstaller
        : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        public override void InstallBindings()
        {
            IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool =
                HierarchyFactory.BuildHierarchyNodeListPool<ILifetimeable>(
                    loggerResolver);

            Container
                .Bind<IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>>>()
                .FromInstance(bufferPool)
                .AsCached();   
                
            var projectLifetime = NonAllocLifetimeFactory.
                BuildNonAllocHierarchicalLifetime(
                    null,
                    bufferPool,
                    loggerResolver: loggerResolver);
            
            projectLifetime.SetUp();

            projectLifetime.Initialize();

            Container
                .Bind<ILifetimeable>()
                .WithId(LifetimeConsts.PROJECT_LIFETIME)
                .FromInstance(projectLifetime)
                .AsCached();
        }

        private void OnDestroy()
        {
            var projectLifetime = Container
                .ResolveId<ILifetimeable>(
                    LifetimeConsts.PROJECT_LIFETIME);

            ITearDownable projectLifetimeAsTearDownable = projectLifetime as ITearDownable;
            
            if (projectLifetimeAsTearDownable != null)
                projectLifetimeAsTearDownable.TearDown();
        }
    }
}
*/