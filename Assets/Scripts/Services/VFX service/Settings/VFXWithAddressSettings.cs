using System;
using UnityEngine.Serialization;

namespace HereticalSolutions.Services.Settings
{
    [Serializable]
    public class VFXWithAddressSettings
    {
        public string VFXAddress;

        public VFXVariantSettings[] Variants;
    }
}