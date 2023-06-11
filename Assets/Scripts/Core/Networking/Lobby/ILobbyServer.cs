namespace HereticalSolutions.Networking
{
    public interface ILobbyServer
    {
        bool LobbyActive { get; }

        bool AllPlayersReady { get; }

        void CreateLobby();

        void CloseLobby();
    }
}