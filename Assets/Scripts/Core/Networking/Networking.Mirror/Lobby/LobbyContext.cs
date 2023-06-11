using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class LobbyContext
    {
        public int CurrentPlayers { get; set; }

        public int MaxPlayers { get; private set; }

        public LobbyContext(
            int maxPlayers)
        {
            CurrentPlayers = 1;
            
            MaxPlayers = maxPlayers;
        }
    }
}