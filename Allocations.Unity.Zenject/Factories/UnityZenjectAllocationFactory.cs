using Zenject;

using UnityEngine;

namespace HereticalSolutions.Allocations.Unity.Zenject.Factories
{
    public static class UnityZenjectAllocationFactory
    {
        public static GameObject DIResolveAllocationDelegate(
            DiContainer container,
            GameObject prefab)
        {
            return container.InstantiatePrefab(prefab);
        }
		
        public static GameObject DIResolveOrInstantiateAllocationDelegate(
            DiContainer container,
            GameObject prefab)
        {
            return container != null
                ? container.InstantiatePrefab(prefab)
                : GameObject.Instantiate(prefab) as GameObject;
        }
    }
}