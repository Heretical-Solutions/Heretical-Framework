using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;
using HereticalSolutions.Repositories;

using Mirror;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Networking.Mirror
{
    public class MirrorLobbyServer
        : ILobbyServer,
          ILobbySlotManager
    {
        #region Message bus

        private readonly INonAllocMessageSender networkingBusAsSender;
        
        private readonly INonAllocMessageReceiver networkingBusAsReceiver;

        #endregion

        #region Subscriptions

        private readonly ISubscription clientConnectedSubscription;
        
        private readonly ISubscription clientDisconnectedSubscription;
        
        private readonly ISubscription clientReadyToReceiveUpdatesSubscription;
        
        private readonly ISubscription clientAddedNewPlayerToServerSubscription;
        
        private readonly ISubscription createLobbyRequestSubscription;
        
        private readonly ISubscription closeLobbyRequestSubscription;

        #endregion

        private bool lobbyActive = false;
        
        private bool allPlayersReady = false;

        private readonly SlotData[] slots;
        
        private readonly IRepository<NetworkConnectionToClient, SlotData> playerToSlotMap;

        private readonly Func<GameObject> slotAllocationDelegate;
        
        private readonly LobbySettings lobbySettings;
        
        
        public MirrorLobbyServer(
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            LobbySettings lobbySettings,
            Func<GameObject> slotAllocationDelegate,
            SlotData[] slots,
            IRepository<NetworkConnectionToClient, SlotData> playerToSlotMap)
        {
            this.networkingBusAsSender = networkingBusAsSender;
            
            this.networkingBusAsReceiver = networkingBusAsReceiver;
            
            this.lobbySettings = lobbySettings;

            this.slotAllocationDelegate = slotAllocationDelegate;
            
            this.slots = slots;

            this.playerToSlotMap = playerToSlotMap;
            
            clientConnectedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientConnectedMessage>(OnClientConnected);
            
            clientDisconnectedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientDisconnectedMessage>(OnClientDisconnected);
            
            clientReadyToReceiveUpdatesSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientReadyToReceiveUpdatesMessage>(OnClientReadyToReceiveUpdates);
            
            clientAddedNewPlayerToServerSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientAddedNewPlayerToServerMessage>(OnClientAddedNewPlayerToServer);
            
            
            createLobbyRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<CreateLobbyRequestMessage>(OnCreateLobbyRequest);
            
            closeLobbyRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<CloseLobbyRequestMessage>(OnCloseLobbyRequest);
            
            networkingBusAsReceiver.SubscribeTo<CreateLobbyRequestMessage>(createLobbyRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<CloseLobbyRequestMessage>(closeLobbyRequestSubscription);
        }

        public void Deinitialize()
        {
            if (createLobbyRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<CreateLobbyRequestMessage>(createLobbyRequestSubscription);
            
            if (closeLobbyRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<CloseLobbyRequestMessage>(closeLobbyRequestSubscription);
        }

        #region ILobbyServer
        
        public bool LobbyActive
        {
            get => lobbyActive;
        }

        public bool AllPlayersReady
        {
            get => allPlayersReady;
        }
        
        public void CreateLobby()
        {
            networkingBusAsReceiver.SubscribeTo<ClientConnectedMessage>(clientConnectedSubscription);
            
            networkingBusAsReceiver.SubscribeTo<ClientDisconnectedMessage>(clientDisconnectedSubscription);
            
            networkingBusAsReceiver.SubscribeTo<ClientReadyToReceiveUpdatesMessage>(clientReadyToReceiveUpdatesSubscription);
            
            networkingBusAsReceiver.SubscribeTo<ClientAddedNewPlayerToServerMessage>(clientAddedNewPlayerToServerSubscription);
            
            lobbyActive = true;
            
            
            networkingBusAsSender
                .PopMessage<StartServerRequestMessage>(out var startServerMessage)
                .SendImmediately<StartServerRequestMessage>(startServerMessage);
            
            networkingBusAsSender
                .PopMessage<StartHostAdvertisingRequestMessage>(out var startAdvertisingMessage)
                .SendImmediately<StartHostAdvertisingRequestMessage>(startAdvertisingMessage);
            
            UnityEngine.Debug.Log($"[MirrorLobbyServer] LOBBY CREATED");
        }

        public void CloseLobby()
        {
            lobbyActive = false;
            
            if (clientConnectedSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientConnectedMessage>(clientConnectedSubscription);
            
            if (clientDisconnectedSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientDisconnectedMessage>(clientDisconnectedSubscription);
            
            if (clientReadyToReceiveUpdatesSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientReadyToReceiveUpdatesMessage>(clientReadyToReceiveUpdatesSubscription);
            
            if (clientAddedNewPlayerToServerSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientAddedNewPlayerToServerMessage>(clientAddedNewPlayerToServerSubscription);
            
            
            networkingBusAsSender
                .PopMessage<StopHostAdvertisingRequestMessage>(out var stopAdvertisingMessage)
                .SendImmediately<StopHostAdvertisingRequestMessage>(stopAdvertisingMessage);
            
            networkingBusAsSender
                .PopMessage<StopServerRequestMessage>(out var stopServerMessage)
                .SendImmediately<StopServerRequestMessage>(stopServerMessage);
            
            UnityEngine.Debug.Log($"[MirrorLobbyServer] LOBBY CLOSED");
        }

        #endregion

        #region ILobbySlotManager

        public bool ClaimSlot(NetworkConnectionToClient connection)
        {
            Debug.Log($"[MirrorLobbyServer] CLAIMING SLOT FOR CLIENT {connection.address}");
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Occupied)
                    continue;
                
                slots[i].Claim(connection);

                playerToSlotMap.Add(
                    connection,
                    slots[i]);
                
                return true;
            }

            Debug.LogError($"[MirrorLobbyServer] NO FREE SLOTS");
            
            return false;
        }

        public bool FreeSlot(NetworkConnectionToClient connection)
        {
            Debug.Log($"[MirrorLobbyServer] FREEING SLOT FOR CLIENT {connection.address}");

            if (!playerToSlotMap.Has(connection))
            {
                Debug.Log($"[MirrorLobbyServer] SLOT NOT FOUND");
                
                return false;
            }

            var slot = playerToSlotMap.Get(connection);
            
            slot.Free();

            playerToSlotMap.Remove(connection);
            
            return true;
        }
        
        public bool RegisterLobbySlotBehaviour(LobbySlotBehaviour slotBehaviour)
        {
            var connection = slotBehaviour.connectionToClient;
            
            Debug.Log($"[MirrorLobbyServer] ADDING LOBBY SLOT BEHAVIOUR FOR CLIENT {connection.address}", slotBehaviour);

            if (!playerToSlotMap.Has(connection))
            {
                Debug.Log($"[MirrorLobbyServer] SLOT NOT FOUND");
                
                return false;
            }

            playerToSlotMap.Get(connection).RegisterBehaviour(slotBehaviour);

            return true;
        }
        
        public bool VacateLobbySlotBehaviour(LobbySlotBehaviour slotBehaviour)
        {
            var connection = slotBehaviour.connectionToClient;
            
            Debug.Log($"[MirrorLobbyServer] REMOVING LOBBY SLOT BEHAVIOUR FOR CLIENT {connection.address}");

            if (!playerToSlotMap.Has(connection))
            {
                Debug.Log($"[MirrorLobbyServer] SLOT NOT FOUND");
                
                return false;
            }

            playerToSlotMap.Get(connection).VacateBehaviour(slotBehaviour);

            return true;
        }
        
        public bool VacateLobbySlotBehaviour(NetworkConnectionToClient connection)
        {
            Debug.Log($"[MirrorLobbyServer] REMOVING LOBBY SLOT BEHAVIOUR FOR CLIENT {connection.address}");

            if (!playerToSlotMap.Has(connection))
            {
                Debug.Log($"[MirrorLobbyServer] SLOT NOT FOUND");
                
                return false;
            }

            playerToSlotMap.Get(connection).VacateBehaviour();

            return true;
        }
        
        public void ReadyStatusChanged()
        {
            Debug.Log($"[MirrorLobbyServer] SOMEONE HAS CHANGED READY STATUS");
            
            int currentPlayers = 0;
            
            int readyPlayers = 0;

            foreach (var slot in slots)
            {
                if (slot.Occupied
                    && slot.Behaviour != null)
                {
                    currentPlayers++;
                    
                    if (slot.Behaviour.ReadyToBegin)
                        readyPlayers++;
                }
            }

            allPlayersReady = (currentPlayers == readyPlayers);
        }

        #endregion
        
        #region Message callbacks
        
        private void OnClientConnected(ClientConnectedMessage message)
        {
            if (DropConnectionIfLobbyInactive(message.Connection))
                return;
            
            Debug.Log($"[MirrorLobbyServer] CLIENT {message.Connection.address} CONNECTED");
        }
        
        private void OnClientReadyToReceiveUpdates(ClientReadyToReceiveUpdatesMessage message)
        {
            if (IgnoreIfLobbyInactive())
                return;
            
            Debug.Log($"[MirrorLobbyServer] CLIENT {message.Connection.address} IS READY TO RECEIVE UPDATES");

            ClaimSlot(message.Connection);
            
            SpawnLobbyPlayerSlot(message.Connection);
        }
        
        private void OnClientAddedNewPlayerToServer(ClientAddedNewPlayerToServerMessage message)
        {
            if (DropConnectionIfLobbyInactive(message.Connection))
                return;
            
            Debug.Log($"[MirrorLobbyServer] CLIENT {message.Connection.address} ADDED NEW PLAYER TO SERVER");
        }

        private void OnClientDisconnected(ClientDisconnectedMessage message)
        {
            if (IgnoreIfLobbyInactive())
                return;
            
            Debug.Log($"[MirrorLobbyServer] CLIENT {message.Connection.address} DISCONNECTED");

            VacateLobbySlotBehaviour(message.Connection);
            
            FreeSlot(message.Connection);
            
            networkingBusAsSender
                .PopMessage<ClientDisconnectionConfirmedMessage>(out var response)
                .Write<ClientDisconnectionConfirmedMessage>(response, new object[] { message.Connection })
                .SendImmediately<ClientDisconnectionConfirmedMessage>(response);
        }

        #endregion

        #region Validation

        private bool IgnoreIfLobbyInactive()
        {
            if (!lobbyActive)
            {
                Debug.LogError("[MirrorLobbyServer] LOBBY IS INACTIVE, IGNORING DROPPED CONNECTION");
            }

            return !lobbyActive;
        }
        
        private bool DropConnectionIfLobbyInactive(NetworkConnectionToClient connection)
        {
            if (!lobbyActive)
            {
                Debug.LogError("[MirrorLobbyServer] LOBBY IS INACTIVE, DROPPING CONNECTION");
                
                connection.Disconnect();
                
                return true;
            }

            return false;
        }

        #endregion

        #region Lobby player slot interaction

        private void SpawnLobbyPlayerSlot(NetworkConnectionToClient connection)
        {
            Debug.Log($"[MirrorLobbyServer] SPAWNING SLOT FOR CLIENT {connection.address}");

            GameObject lobbyPlayerSlotGameObject = slotAllocationDelegate.Invoke();

            NetworkServer.AddPlayerForConnection(
                connection,
                lobbyPlayerSlotGameObject);
        }

        /*
        private void RemoveLobbyPlayerSlot(NetworkConnectionToClient connection)
        {
            if (connection.identity != null)
            {
                Debug.Log($"[MirrorLobbyServer] REMOVING SLOT FOR CLIENT {connection.address}");
                
                LobbyPlayerSlot slot = connection.identity.GetComponent<LobbyPlayerSlot>();

                if (slot != null)
                    playerToSlotMap.Get(connection).SlotDespawned(slot);

                foreach (NetworkIdentity clientOwnedObject in connection.owned)
                {
                    slot = clientOwnedObject.GetComponent<LobbyPlayerSlot>(); //FFS, GETCOMPONENT IN RUNTIME?
                    
                    if (slot != null)
                        lobbySlots.Remove(slot);
                }
            }
        }
        */
        
        private void SetAllPlayersToNotReady()
        {
            Debug.Log($"[MirrorLobbyServer] SETTING ALL PLAYERS TO NOT READY");
            
            foreach (var slot in slots)
            {
                if (slot.Behaviour != null)
                    slot.Behaviour.ReadyToBegin = false;
            }
        }
        
        #endregion

        /*
        private void CreateLobbyPlayerData(NetworkConnectionToClient connection)
        {
            if (connection != null
                && connection.identity != null)
            {
                GameObject identityGameObject = connection.identity.gameObject;

                // if null or not a room player, don't replace it
                if (identityGameObject != null
                    && identityGameObject.GetComponent<LobbyPlayerSlot>() != null)
                {
                    Debug.Log($"[MirrorLobbyServer] CREATING LOBBY PLAYER DATA FOR CLIENT {connection.address}");
                    
                    // cant be ready in room, add to ready list
                    playerToSlotMap.Get(connection).PlayerConnected(
                        connection,
                        identityGameObject);
                }
            }
        }
        */

        /*
        /// <summary>
        /// CheckReadyToBegin checks all of the players in the room to see if their readyToBegin flag is set.
        /// <para>If all of the players are ready, then the server switches from the RoomScene to the PlayScene, essentially starting the game. This is called automatically in response to NetworkRoomPlayer.CmdChangeReadyState.</para>
        /// </summary>
        public void CheckReadyToBegin()
        {
            int numberOfReadyPlayers = NetworkServer.connections.Count(conn =>
                conn.Value != null &&
                conn.Value.identity != null &&
                conn.Value.identity.TryGetComponent(out NetworkRoomPlayer nrp) &&
                nrp.readyToBegin);

            bool enoughReadyPlayers = numberOfReadyPlayers == lobbySettings.MaxPlayers;
            
            if (enoughReadyPlayers)
            {
                pendingPlayers.Clear();
                allPlayersReady = true;
            }
            else
                allPlayersReady = false;
        }
        */
        
        private void OnCreateLobbyRequest(CreateLobbyRequestMessage message)
        {
            CreateLobby();
        }
        
        private void OnCloseLobbyRequest(CloseLobbyRequestMessage message)
        {
            CloseLobby();
        }
    }
}