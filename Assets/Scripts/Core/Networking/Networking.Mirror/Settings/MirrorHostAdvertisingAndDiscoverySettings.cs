using HereticalSolutions.Allocations.Factories;

using UnityEngine;

namespace HereticalSolutions.Networking.Mirror
{
    [CreateAssetMenu(fileName = "Host advertising and discovery settings", menuName = "Settings/Networking/Mirror host advertising and discovery settings", order = 0)]
    public class MirrorHostAdvertisingAndDiscoverySettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("If true, broadcasts a discovery request every ActiveDiscoveryInterval seconds")]
        public bool EnableActiveDiscovery = true;

        // broadcast address needs to be configurable on iOS:
        // https://github.com/vis2k/Mirror/pull/3255
        [Tooltip("iOS may require LAN IP address here (e.g. 192.168.x.x), otherwise leave blank.")]
        public string BroadcastAddress = "";

        [SerializeField]
        [Tooltip("The UDP port the server will listen for multi-cast messages")]
        public int ServerBroadcastListenPort = 47777;

        [SerializeField]
        [Tooltip("Time in seconds between multi-cast messages")]
        [Range(1, 60)]
        public float ActiveDiscoveryInterval = 3;
        
        // Each game should have a random unique handshake,
        // this way you can tell if this is the same game or not
        [SerializeField]
        public long SecretHandshake;
        
        [SerializeField]
        public bool ValidateHandshakeInEditor = false;
        
#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            if (ValidateHandshakeInEditor
                && SecretHandshake == 0)
            {
                SecretHandshake = IDAllocationsFactory.BuildLongFromTwoRandomInts();
            }
        }
#endif
    }
}