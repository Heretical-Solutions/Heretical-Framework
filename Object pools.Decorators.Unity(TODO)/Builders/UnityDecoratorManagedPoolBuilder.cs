using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Unity.Factories;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity.Builders
{
	public static class UnityDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<GameObject> DecoratedWithUnityPool(
			this ManagedPoolBuilder<GameObject> builder,
			UnityDecoratorPoolFactory unityDecoratorPoolFactory,
			Transform parentTransform = null)
		{
			var context = builder.Context;

			context.FinalBuildSteps.Add(
				(delegateContext) =>
				{
					// Build the game object pool
					var gameObjectPool = unityDecoratorPoolFactory.
						BuildGameObjectDecoratorManagedPool(
							delegateContext.CurrentPool,
							parentTransform);

					delegateContext.CurrentPool = gameObjectPool;
				});

			return builder;
		}

		public static ManagedPoolBuilder<GameObject>
			WithPrefab(
				this ManagedPoolBuilder<GameObject> builder,
				UnityDecoratorAllocationCallbackFactory 
					unityDecoratorAllocationCallbackFactory,
				GameObject prefab)
		{
			var context = builder.Context;

			SetPrefabAllocationCallback setPrefabCallback =
				unityDecoratorAllocationCallbackFactory.
					BuildSetPrefabAllocationCallback(
						prefab);

			context.FacadeAllocationCallbacks.Add(
				setPrefabCallback);

			return builder;
		}
	}
}