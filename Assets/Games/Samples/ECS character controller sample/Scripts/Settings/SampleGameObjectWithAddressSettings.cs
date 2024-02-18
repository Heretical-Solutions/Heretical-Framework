using System;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample
{
	[Serializable]
	public class SampleGameObjectWithAddressSettings
	{
		public string GameObjectAddress;

		public SampleGameObjectVariantSettings[] Variants;
	}
}