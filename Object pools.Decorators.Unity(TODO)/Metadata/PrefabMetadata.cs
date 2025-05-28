using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity
{
	public class PrefabMetadata
		: IContainsPrefab
	{
		public PrefabMetadata()
		{
			Prefab = null;
		}

		#region IContainsPrefab

		public GameObject Prefab { get; set; }

		#endregion

	}
}