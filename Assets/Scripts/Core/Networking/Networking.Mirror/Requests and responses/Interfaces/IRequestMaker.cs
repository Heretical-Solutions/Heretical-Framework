using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public interface IRequestMaker<TRequest>
        where TRequest : NetworkMessage
    {
        TRequest MakeRequest(NetworkingContext context);
    }
}