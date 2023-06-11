using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public interface ILobbySlotManager
    {
        bool ClaimSlot(NetworkConnectionToClient connection);

        bool FreeSlot(NetworkConnectionToClient connection);
        
        bool RegisterLobbySlotBehaviour(LobbySlotBehaviour slotBehaviour);

        bool VacateLobbySlotBehaviour(LobbySlotBehaviour slotBehaviour);
        
        bool VacateLobbySlotBehaviour(NetworkConnectionToClient connection);

        void ReadyStatusChanged();
    }
}