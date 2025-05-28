using HereticalSolutions.ObjectPools.Managed;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity.Factories
{
	public class UnityDecoratorPoolFactory
	{
		public GameObjectDecoratorPool BuildGameObjectDecoratorPool(
			IPool<GameObject> innerPool,
			Transform parentTransform = null)
		{
			return new GameObjectDecoratorPool(
				innerPool,
				parentTransform);
		}

		public GameObjectDecoratorManagedPool BuildGameObjectDecoratorManagedPool(
			IManagedPool<GameObject> innerPool,
			Transform parentTransform = null)
		{
			return new GameObjectDecoratorManagedPool(
				innerPool,
				parentTransform);
		}
	}
}