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

				var resolveMeAs = gameObjectWithTag.GetComponent<ResolveMeAs>();

				if (resolveMeAs == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NO ResolveMeAs COMPONENT",
						new object[]
						{
							gameObjectWithTag
						});

					continue;
				}

				var sceneEntity = gameObjectWithTag.GetComponent<SampleSceneEntity>();

				if (sceneEntity == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NO SampleSceneEntity COMPONENT",
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

				entityManager.ResolveEntity(
					sceneEntity.EntityID,
					gameObjectWithTag,
					resolveMeAs.PrototypeID);

				ResolveChildren(
					sceneEntity,
					logger);
			}
		}

		private void ResolveChildren(
			SampleSceneEntity parentSceneEntity,
			ILogger logger = null)
		{
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

				var childResolveMeAs = childSceneEntity.GetComponent<ResolveMeAs>();

				if (childResolveMeAs == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {childSceneEntity.name} HAS AN ENTITY TAG BUT NO ResolveMeAs COMPONENT",
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

				entityManager.ResolveEntity(
					childSceneEntity.EntityID,
					childSceneEntity.gameObject,
					childResolveMeAs.PrototypeID);
			}

		}
	}
}