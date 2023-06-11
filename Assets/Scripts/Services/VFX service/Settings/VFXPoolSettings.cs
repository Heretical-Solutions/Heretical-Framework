using UnityEngine;

namespace HereticalSolutions.Services.Settings
{
    [CreateAssetMenu(fileName = "VFX pool settings", menuName = "Settings/Object pools/VFX pool settings", order = 0)]
    public class VFXPoolSettings : ScriptableObject
    {
        public string PoolID;

        public VFXWithAddressSettings[] Elements;
    }
}