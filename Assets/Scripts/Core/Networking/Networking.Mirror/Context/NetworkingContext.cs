using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class NetworkingContext
    {
        public long ServerId { get; private set; }

        public string Hostname { get; private set; }

        public Transport Transport { get; private set; }

        public NetworkingContext(
            long serverId,
            Transport transport,
            string hostname)
        {
            ServerId = serverId;

            Transport = transport;

            Hostname = hostname;
        }
    }
}