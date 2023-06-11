using Mirror;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Networking.Mirror
{
    /// <summary>
    /// This component works in conjunction with the NetworkRoomManager to make up the multiplayer room system.
    /// <para>The RoomPrefab object of the NetworkRoomManager must have this component on it. This component holds basic room player data required for the room to function. Game specific data for room players can be put in other components on the RoomPrefab or in scripts derived from NetworkRoomPlayer.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public class LobbySlotBehaviour : NetworkBehaviour
    {
        [Inject]
        private ILobbyServer lobbyServer;
        
        [Inject]
        private ILobbySlotManager lobbySlotManager;
        
        /// <summary>
        /// Diagnostic flag indicating whether this player is ready for the game to begin.
        /// <para>Invoke CmdChangeReadyState method on the client to set this flag.</para>
        /// <para>When all players are ready to begin, the game will start. This should not be set directly, CmdChangeReadyState should be called on the client to set it on the server.</para>
        /// </summary>
        [SyncVar(hook = nameof(ReadyStateChanged))]
        private bool readyToBegin;

        public bool ReadyToBegin
        {
            get => readyToBegin;
            set => readyToBegin = value;
        }

        /// <summary>
        /// Diagnostic index of the player, e.g. Player1, Player2, etc.
        /// </summary>
        [SyncVar(hook = nameof(IndexChanged))]
        private int indexInLobby;

        public int IndexInLobby
        {
            get => indexInLobby;
            set => indexInLobby = value;
        }

        #region Unity Callbacks

        /// <summary>
        /// Do not use Start - Override OnStartHost / OnStartClient instead!
        /// </summary>
        public void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            Debug.Log($"[LobbyPlayerSlot] INITIALIZING", this);
            
            DontDestroyOnLoad(gameObject);

            if (NetworkServer.active
                && lobbyServer.LobbyActive)
                lobbySlotManager.RegisterLobbySlotBehaviour(this);
        }

        public void OnDisable()
        {
            Debug.Log($"[LobbyPlayerSlot] DEINITIALIZING", this);

            if (NetworkServer.active
                && lobbyServer.LobbyActive)
            {
                lobbySlotManager.VacateLobbySlotBehaviour(this);

                lobbySlotManager.FreeSlot(connectionToClient);
            }
        }

        #endregion

        #region Commands

        [Command]
        public void CmdChangeReadyState(bool value)
        {
            Debug.Log($"[LobbyPlayerSlot] CHANGING READY STATE TO {value}");
            
            readyToBegin = value;
            
            if (lobbyServer.LobbyActive)
                lobbySlotManager.ReadyStatusChanged();
        }

        #endregion

        #region SyncVar Hooks

        /// <summary>
        /// This is a hook that is invoked on clients when the index changes.
        /// </summary>
        /// <param name="oldIndex">The old index value</param>
        /// <param name="newIndex">The new index value</param>
        private void IndexChanged(
            int oldIndex,
            int newIndex)
        {
            Debug.Log($"[LobbyPlayerSlot] INDEX HAS CHANGED TO {newIndex}", this);
        }

        /// <summary>
        /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
        /// <para>This function is called when the a client player calls CmdChangeReadyState.</para>
        /// </summary>
        /// <param name="newReadyState">New Ready State</param>
        private void ReadyStateChanged(
            bool oldReadyState,
            bool newReadyState)
        {
            Debug.Log($"[LobbyPlayerSlot] READY STATE HAS CHANGED TO {newReadyState}", this);
        }

        #endregion
    }
}