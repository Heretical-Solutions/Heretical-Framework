using System;

using HereticalSolutions.Pools;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

using Zenject;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample.Installers
{
	public class SampleSystemsInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private SampleEntityManager entityManager;

		[Inject]
		private INonAllocDecoratedPool<GameObject> gameObjectPool;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<SampleEntityPrototypeImportInstaller>();

			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

			var entityWorldsRepository = worldContainer.EntityWorldsRepository;

			var viewWorldController = entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

			var viewWorldSystemsContainer = viewWorldController as  IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem>;

			viewWorldSystemsContainer.Initialize(
				new DefaultECSSequentialEntityInitializationSystem(
					new ResolvePooledGameObjectViewSystem<SampleSceneEntity>(
						gameObjectPool,
						loggerResolver?.GetLogger<ResolvePooledGameObjectViewSystem<SampleSceneEntity>>())),
				new DefaultECSSequentialEntityInitializationSystem(
					new SpawnPooledGameObjectViewSystem(
						gameObjectPool,
						loggerResolver?.GetLogger<SpawnPooledGameObjectViewSystem>())),
				null);
		}
	}
}