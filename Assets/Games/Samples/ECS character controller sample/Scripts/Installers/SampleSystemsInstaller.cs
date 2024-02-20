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
		private IEntityManager<World, Guid, Entity> entityManager;

		[Inject]
		private INonAllocDecoratedPool<GameObject> gameObjectPool;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<SampleEntityPrototypeImportInstaller>();

			var worldContainer = entityManager as IContainsEntityWorlds<World, ISystem<Entity>, Entity>;

			var entityWorldsRepository = worldContainer.EntityWorldsRepository;

			var viewWorldController = entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

			var viewWorldSystemsContainer = viewWorldController as  IContainsEntityInitializationSystems<ISystem<Entity>>;

			viewWorldSystemsContainer.Initialize(
				new SequentialSystem<Entity>(
					new ResolvePooledGameObjectViewSystem(
						gameObjectPool,
						loggerResolver?.GetLogger<ResolvePooledGameObjectViewSystem>())),
				new SequentialSystem<Entity>(
					new SpawnPooledGameObjectViewSystem(
						gameObjectPool,
						loggerResolver?.GetLogger<SpawnPooledGameObjectViewSystem>())),
				null);
		}
	}
}