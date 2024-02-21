using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleResolveEntitiesOnSceneInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private SampleEntityManager entityManager;

		public override void InstallBindings()
		{
			var logger = loggerResolver?.GetLogger<SampleResolveEntitiesOnSceneInstaller>();

			var gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Entity");

			foreach (var gameObjectWithTag in gameObjectsWithTag)
			{
				var adapter = gameObjectWithTag.GetComponent<GameObjectViewEntityAdapter>();

				if (adapter == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NO GameObjectViewEntityAdapter COMPONENT",
						new object[]
						{
							gameObjectWithTag
						});

					continue;
				}

				var sceneEntity = gameObjectWithTag.GetComponent<SampleSceneEntity>();

				var worldLocalSceneEntity = gameObjectWithTag.GetComponent<WorldLocalSceneEntity>();

				if (sceneEntity == null
					&& worldLocalSceneEntity == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NEITHER SampleSceneEntity NOR WorldLocalSceneEntity COMPONENT",
						new object[]
						{
							gameObjectWithTag
						});

					continue;
				}

				logger?.Log<SampleResolveEntitiesOnSceneInstaller>(
					$"RESOLVING GAME OBJECT {gameObjectWithTag.name}",
					new object[]
					{
						gameObjectWithTag
					});

				if (sceneEntity != null)
				{
					entityManager.ResolveEntity(
						sceneEntity.EntityID,
						gameObjectWithTag,
						sceneEntity.PrototypeID);

					ResolveChildren(
						sceneEntity,
						logger);
				}

				if (worldLocalSceneEntity != null)
				{
					entityManager.ResolveWorldLocalEntity(
						worldLocalSceneEntity.PrototypeID,
						gameObjectWithTag,
						worldLocalSceneEntity.WorldID);

					ResolveChildren(
						worldLocalSceneEntity,
						logger);
				}
			}
		}

		private void ResolveChildren(
			ASceneEntity parentSceneEntity,
			ILogger logger = null)
		{
			if (parentSceneEntity.childEntities == null)
				return;

			foreach (var childSceneEntity in parentSceneEntity.childEntities)
			{
				var childAdapter = childSceneEntity.GetComponent<GameObjectViewEntityAdapter>();

				if (childAdapter == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {childSceneEntity.name} HAS AN ENTITY TAG BUT NO ViewEntityAdapter COMPONENT",
						new object[]
						{
							childSceneEntity
						});

					continue;
				}

				logger?.Log<SampleResolveEntitiesOnSceneInstaller>(
					$"RESOLVING GAME OBJECT {childSceneEntity.gameObject.name}",
					new object[]
					{
						childSceneEntity.gameObject
					});

				var childSampleSceneEntity = childSceneEntity as SampleSceneEntity;

				if (childSampleSceneEntity != null)
				{
					entityManager.ResolveEntity(
						childSampleSceneEntity.EntityID,
						childSceneEntity.gameObject,
						childSampleSceneEntity.PrototypeID);
				}

				var childWorldLocalSceneEntity = childSceneEntity as WorldLocalSceneEntity;

				if (childWorldLocalSceneEntity != null)
				{
					entityManager.ResolveWorldLocalEntity(
						childWorldLocalSceneEntity.PrototypeID,
						childWorldLocalSceneEntity.gameObject,
						childWorldLocalSceneEntity.WorldID);
				}
			}

		}
	}
}