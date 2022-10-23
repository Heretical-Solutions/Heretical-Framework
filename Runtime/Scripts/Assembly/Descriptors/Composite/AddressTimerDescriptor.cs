using UnityEngine;

namespace HereticalSolutions.Assembly.Descriptors
{
	[System.Serializable]
	public struct AddressTimerDescriptor
	{
		public string Address;

		public float DefaultDuration;

		public GameObject Prefab;
	}
}