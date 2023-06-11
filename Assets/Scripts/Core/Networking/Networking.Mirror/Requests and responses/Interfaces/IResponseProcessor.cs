using System.Net;

namespace HereticalSolutions.Networking.Mirror
{
    public interface IResponseProcessor<TResponse>
    {
        void ProcessResponse(
            TResponse response,
            IPEndPoint endpoint,
            NetworkingContext context);
    }
}