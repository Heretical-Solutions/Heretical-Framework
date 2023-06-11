using HereticalSolutions.Messaging;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class ClientConnectedMessage : IMessage
    {
        public NetworkConnectionToClient Connection { get; private set; }

        public void Write(object[] args)
        {
            Connection = (NetworkConnectionToClient)args[0];
        }
    }
}