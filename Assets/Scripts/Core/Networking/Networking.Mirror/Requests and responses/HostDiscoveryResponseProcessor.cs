using System;
using System.Net;

using HereticalSolutions.Messaging;

namespace HereticalSolutions.Networking.Mirror
{
    public class HostDiscoveryResponseProcessor : IResponseProcessor<MirrorHostDiscoveryRequestResponse>
    {
        private INonAllocMessageSender networkingBus;

        public HostDiscoveryResponseProcessor(INonAllocMessageSender networkingBus)
        {
            this.networkingBus = networkingBus;
        }

        public void ProcessResponse(
            MirrorHostDiscoveryRequestResponse response,
            IPEndPoint endpoint,
            NetworkingContext context)
        {
            UnityEngine.Debug.Log($"[HostDiscoveryRequestMaker] PROCESSING HOST DISCOVERY RESPONSE. SERVER ID: {response.ServerId} URI: {response.URI}");
            
            // we received a message from the remote endpoint
            //response.EndPoint = endpoint;

            // although we got a supposedly valid url, we may not be able to resolve
            // the provided host
            // However we know the real ip address of the server because we just
            // received a packet from it,  so use that as host.
            UriBuilder realUri = new UriBuilder(response.URI)
            {
                Host = endpoint.Address.ToString() //response.EndPoint.Address.ToString()
            };
            
            //response.URI = realUri.Uri;

            //OnServerFound.Invoke(response);
            
            UnityEngine.Debug.Log("[HostDiscoveryRequestMaker] RESPONSE PROCESSED, SENDING MESSAGE TO NETWORKING BUS");
            
            networkingBus
                .PopMessage<HostDiscoveredMessage>(out var message)
                .Write<HostDiscoveredMessage>(
                    message,
                    new object[]
                    {
                        response.ServerId,
                        realUri.Uri,
                        endpoint
                    })
                .SendImmediately<HostDiscoveredMessage>(message);
        }
    }
}