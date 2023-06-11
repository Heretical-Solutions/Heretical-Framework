using UnityEngine;

namespace HereticalSolutions.Networking.Mirror
{
    public class HostDiscoveryRequestMaker : IRequestMaker<MirrorHostDiscoveryRequest>
    {
        public MirrorHostDiscoveryRequest MakeRequest(NetworkingContext context)
        {
            Debug.Log("[HostDiscoveryRequestMaker] MAKING HOST DISCOVERY REQUEST");
            
            return new MirrorHostDiscoveryRequest();
        }
    }
}