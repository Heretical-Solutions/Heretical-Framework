using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity
{
	public interface IContainsPrefab
	{
		GameObject Prefab { get; set; }
	}
}