using UnityEngine;

namespace HereticalSolutions.Assembly.Descriptors
{
	[System.Serializable]
	public struct VariantTimerDescriptor
	{
		public float Chance;

		public float DefaultDuration;

		public GameObject Prefab;
	}
}