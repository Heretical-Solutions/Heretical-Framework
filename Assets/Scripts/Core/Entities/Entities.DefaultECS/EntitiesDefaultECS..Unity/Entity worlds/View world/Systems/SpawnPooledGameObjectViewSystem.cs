using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Entities
{
	public class SpawnPooledGameObjectViewSystem : ISystem<Entity>
	{
		private readonly INonAllocDecoratedPool<GameObject> pool;


		private readonly AddressArgument addressArgument;

		private readonly IPoolDecoratorArgument[] arguments;


		private readonly ILogger logger;

		public SpawnPooledGameObjectViewSystem(
			INonAllocDecoratedPool<GameObject> pool,
			ILogger logger = null)
		{
			this.pool = pool;

			this.logger = logger;

			addressArgument = new AddressArgument();

			arguments = new IPoolDecoratorArgument[]
			{
				addressArgument
			};
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		void ISystem<Entity>.Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<SpawnPooledGameObjectView>())
				return;


			ref SpawnPooledGameObjectView spawnViewComponent = ref entity.Get<SpawnPooledGameObjectView>();

			string address = spawnViewComponent.Address;


			addressArgument.FullAddress = address;
			addressArgument.AddressHashes = address.AddressToHashes();

			var pooledViewElement = pool
				.Pop(arguments);


			var pooledGameObjectViewComponent = new PooledGameObjectView();

			pooledGameObjectViewComponent.Element = pooledViewElement;

			entity.Set<PooledGameObjectView>(pooledGameObjectViewComponent);


			entity.Remove<SpawnPooledGameObjectView>();


			var viewEntityAdapter = pooledViewElement.Value.GetComponent<GameObjectViewEntityAdapter>();

			if (viewEntityAdapter != null)
			{
				viewEntityAdapter.Initialize(entity);
			}
			else
			{
				logger?.LogError(
					$"POOL ELEMENT {pooledViewElement.Value.name} HAS NO GameObjectViewEntityAdapter", new [] { pooledViewElement.Value });
			}
		}

		public void Dispose()
		{
		}
	}
}