using System;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Entities
{
	public class ResolvePooledGameObjectViewSystem : IDefaultECSEntityInitializationSystem
	{
		private readonly INonAllocDecoratedPool<GameObject> pool;

		private readonly AddressArgument addressArgument;
		private readonly AppendArgument appendArgument;
		private readonly IPoolDecoratorArgument[] arguments;

		private readonly ILogger logger;

		public ResolvePooledGameObjectViewSystem(
			INonAllocDecoratedPool<GameObject> pool,
			ILogger logger = null)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();
			appendArgument = new AppendArgument();

			arguments = new IPoolDecoratorArgument[]
			{
				addressArgument,
				appendArgument
			};
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<ResolveViewComponent>())
				return;

			if (!entity.Has<SpawnPooledGameObjectView>())
			{
				return;

				//throw new Exception(
				//	logger.FormatException(
				//		$"ENTITY {entity.Get<GUIDComponent>().GUID} WAS REQUESTED TO BE RESOLVED BUT HAS NO SpawnPooledGameObjectView"));
			}

			ref ResolveViewComponent resolveViewComponent = ref entity.Get<ResolveViewComponent>();

			ref SpawnPooledGameObjectView spawnViewComponent = ref entity.Get<SpawnPooledGameObjectView>();

			string address = spawnViewComponent.Address;


			addressArgument.FullAddress = address;
			addressArgument.AddressHashes = address.AddressToHashes();

			var pooledViewElement = pool
				.Pop(arguments);


			pooledViewElement.Value = (GameObject)resolveViewComponent.Source;


			var pooledGameObjectViewComponent = new PooledGameObjectView();

			pooledGameObjectViewComponent.Element = pooledViewElement;

			entity.Set<PooledGameObjectView>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectView>();


			var sceneEntity = pooledViewElement.Value.GetComponent<SceneEntity>();

			if (sceneEntity != null)
				GameObject.Destroy(sceneEntity);


			var viewEntityAdapter = pooledViewElement.Value.GetComponent<GameObjectViewEntityAdapter>();

			if (viewEntityAdapter != null)
			{
				viewEntityAdapter.Initialize(entity);
			}
			else
			{
				logger?.LogError(
					$"POOL ELEMENT {pooledViewElement.Value.name} HAS NO GameObjectViewEntityAdapter", new[] { pooledViewElement.Value });
			}
		}

		/// <summary>
		/// Disposes the system.
		/// </summary>
		public void Dispose()
		{
		}
	}
}