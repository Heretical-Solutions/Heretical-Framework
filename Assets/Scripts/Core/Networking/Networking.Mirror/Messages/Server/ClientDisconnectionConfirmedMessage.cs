using HereticalSolutions.Messaging;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class ClientDisconnectionConfirmedMessage : IMessage
    {
        public NetworkConnectionToClient Connection { get; private set; }

        public void Write(object[] args)
        {
            Connection = (NetworkConnectionToClient)args[0];
        }
    }
}