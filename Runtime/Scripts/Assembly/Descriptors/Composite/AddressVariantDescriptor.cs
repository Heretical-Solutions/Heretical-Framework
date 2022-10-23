using UnityEngine;

namespace HereticalSolutions.Assembly.Descriptors
{
	[System.Serializable]
	public struct AddressVariantDescriptor
	{
		public string Address;

		public VariantDescriptor[] Variants;
	}
}