using UnityEngine;

namespace HereticalSolutions.Allocations.Unity.Factories
{
	public static class UnityAllocationsFactory
	{
		public static GameObject InstantiatePrefabAllocationDelegate(
			GameObject prefab)
		{
			return GameObject.Instantiate(prefab) as GameObject;
		}
	}
}