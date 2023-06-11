using System;
using UnityEngine.Serialization;

namespace HereticalSolutions.Services.Settings
{
    [Serializable]
    public class GameObjectWithAddressSettings
    {
        public string GameObjectAddress;

        public GameObjectVariantSettings[] Variants;
    }
}