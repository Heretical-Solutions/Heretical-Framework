using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class MirrorLobbyClient : ILobbyClient
    {
        #region Message bus

        private readonly INonAllocMessageSender networkingBusAsSender;
        
        private readonly INonAllocMessageReceiver networkingBusAsReceiver;

        #endregion

        #region Subscriptions
        
        private readonly ISubscription clientStartedSubscription;
        
        private readonly ISubscription joinLobbyRequestSubscription;
        
        private readonly ISubscription leaveLobbyRequestSubscription;
        
        #endregion
        
        private readonly LobbySettings lobbySettings;

        private bool clientActive = false;
        
        public MirrorLobbyClient(
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            LobbySettings lobbySettings)
        {
            this.networkingBusAsSender = networkingBusAsSender;
            
            this.networkingBusAsReceiver = networkingBusAsReceiver;

            this.lobbySettings = lobbySettings;
            
            clientStartedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientStartedMessage>(OnClientStarted);
            
            
            joinLobbyRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<JoinLobbyRequestMessage>(OnJoinLobbyRequest);
            
            leaveLobbyRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<LeaveLobbyRequestMessage>(OnLeaveLobbyRequest);
            
            networkingBusAsReceiver.SubscribeTo<JoinLobbyRequestMessage>(joinLobbyRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<LeaveLobbyRequestMessage>(leaveLobbyRequestSubscription);
        }

        public void Deinitialize()
        {
            if (joinLobbyRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<JoinLobbyRequestMessage>(joinLobbyRequestSubscription);
            
            if (leaveLobbyRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<LeaveLobbyRequestMessage>(leaveLobbyRequestSubscription);
        }

        public bool ClientActive
        {
            get => clientActive;
        }

        public void JoinLobby(Uri uri)
        {
            networkingBusAsReceiver.SubscribeTo<ClientStartedMessage>(clientStartedSubscription);
            
            clientActive = true;
            
            
            networkingBusAsSender
                .PopMessage<StartClientRequestMessage>(out var startClientMessage)
                .Write<StartClientRequestMessage>(startClientMessage, new object[] { uri })
                .SendImmediately<StartClientRequestMessage>(startClientMessage);
            
            UnityEngine.Debug.Log($"[MirrorLobbyClient] LOBBY JOINED");
        }

        public void LeaveLobby()
        {
            clientActive = false;
            
            if (clientStartedSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientStartedMessage>(clientStartedSubscription);

            networkingBusAsSender
                .PopMessage<StopClientRequestMessage>(out var stopClientMessage)
                .SendImmediately<StopClientRequestMessage>(stopClientMessage);
            
            UnityEngine.Debug.Log($"[MirrorLobbyClient] LOBBY LEFT");
        }

        private void OnClientStarted(ClientStartedMessage message)
        {
            UnityEngine.Debug.Log($"[MirrorLobbyClient] CLIENT STARTED");
            
            if (lobbySettings.lobbySlotBehaviourPrefab == null
                || lobbySettings.lobbySlotBehaviourPrefab.gameObject == null)
                UnityEngine.Debug.LogError("NetworkRoomManager no RoomPlayer prefab is registered. Please add a RoomPlayer prefab.");
            else
                NetworkClient.RegisterPrefab(lobbySettings.lobbySlotBehaviourPrefab.gameObject);
        }
        
        private void OnJoinLobbyRequest(JoinLobbyRequestMessage message)
        {
            JoinLobby(message.URI);
        }
        
        private void OnLeaveLobbyRequest(LeaveLobbyRequestMessage message)
        {
            LeaveLobby();
        }
    }
}