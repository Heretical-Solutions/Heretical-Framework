using System;

namespace HereticalSolutions.Networking
{
    public interface ILobbyClient
    {
        bool ClientActive { get; }

        void JoinLobby(Uri uri);

        void LeaveLobby();
    }
}