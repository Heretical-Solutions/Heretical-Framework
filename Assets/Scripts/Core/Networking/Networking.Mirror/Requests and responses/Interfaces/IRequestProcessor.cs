using System.Net;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public interface IRequestProcessor<TRequest, TResponse>
        where TRequest : NetworkMessage
        where TResponse : NetworkMessage
    {
        bool ProcessRequest(
            TRequest request,
            IPEndPoint endpoint,
            NetworkingContext context,
            out TResponse response);
    }
}