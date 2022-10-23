using UnityEngine;

namespace HereticalSolutions.Assembly.Descriptors
{
	[System.Serializable]
	public struct AddressVariantTimerDescriptor
	{
		public string Address;

		public VariantTimerDescriptor[] Variants;
	}
}