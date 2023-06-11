using System;
using System.Net;

namespace HereticalSolutions.Networking.Mirror
{
    public class HostDiscoveryRequestProcessor : IRequestProcessor<MirrorHostDiscoveryRequest, MirrorHostDiscoveryRequestResponse>
    {
        public bool ProcessRequest(
            MirrorHostDiscoveryRequest request,
            IPEndPoint endpoint,
            NetworkingContext context,
            out MirrorHostDiscoveryRequestResponse response)
        {
            UnityEngine.Debug.Log("[HostDiscoveryRequestMaker] PROCESSING HOST DISCOVERY REQUEST");
            
            // In this case we don't do anything with the request
            // but other discovery implementations might want to use the data
            // in there,  This way the client can ask for
            // specific game mode or something

            try
            {
                // this is an example reply message,  return your own
                // to include whatever is relevant for your game
                response = new MirrorHostDiscoveryRequestResponse
                {
                    ServerId = context.ServerId,
                    
                    URI = context.Transport.ServerUri()
                };
                
                UnityEngine.Debug.Log($"[HostDiscoveryRequestMaker] REQUEST PROCESSED, RESPONSE CREATED. SERVER ID: {response.ServerId} URI: {response.URI}");

                return true;
            }
            catch (NotImplementedException)
            {
                UnityEngine.Debug.LogError($"Transport {context.Transport} does not support network discovery");
                
                throw;
            }
        }
    }
}