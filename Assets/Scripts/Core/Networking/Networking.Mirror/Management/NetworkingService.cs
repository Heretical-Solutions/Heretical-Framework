using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using Mirror;

namespace HereticalSolutions.Networking.Mirror
{
    public class NetworkingService : NetworkManager
    {
        #region Message bus
        
        private INonAllocMessageSender networkingBusAsSender;
        
        private INonAllocMessageReceiver networkingBusAsReceiver;

        #endregion
        
        #region Subscriptions
        
        private ISubscription clientDisconnectionConfirmedSubscription;
        
        private ISubscription startServerRequestSubscription;
        
        private ISubscription stopServerRequestSubscription;

        private ISubscription startClientRequestSubscription;
        
        private ISubscription stopClientRequestSubscription;
        
        #endregion
        
        public void Initialize(
            INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver)
        {
            this.networkingBusAsSender = networkingBusAsSender;

            this.networkingBusAsReceiver = networkingBusAsReceiver;
            
            clientDisconnectionConfirmedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientDisconnectionConfirmedMessage>(OnClientDisconnectionConfirmed);
            
            startServerRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StartServerRequestMessage>(OnStartServerRequest);
            
            stopServerRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StopServerRequestMessage>(OnStopServerRequest);
            
            startClientRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StartClientRequestMessage>(OnStartClientRequest);
            
            stopClientRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StopClientRequestMessage>(OnStopClientRequest);
            
            networkingBusAsReceiver.SubscribeTo<ClientDisconnectionConfirmedMessage>(clientDisconnectionConfirmedSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StartServerRequestMessage>(startServerRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StopServerRequestMessage>(stopServerRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StartClientRequestMessage>(startClientRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StopClientRequestMessage>(stopClientRequestSubscription);
        }

        public void Deinitialize()
        {
            if (clientDisconnectionConfirmedSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<ClientDisconnectionConfirmedMessage>(clientDisconnectionConfirmedSubscription);
            
            if (startServerRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StartServerRequestMessage>(startServerRequestSubscription);
            
            if (stopServerRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StopServerRequestMessage>(stopServerRequestSubscription);
            
            if (startClientRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StartClientRequestMessage>(startClientRequestSubscription);
            
            if (stopClientRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StopClientRequestMessage>(stopClientRequestSubscription);
        }

        public override void OnDestroy()
        {
            Deinitialize();
            
            base.OnDestroy();
        }

        /// <summary>
        /// Called on the server when a new client connects.
        /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
        /// </summary>
        /// <param name="connection">Connection from client.</param>
        public override void OnServerConnect(NetworkConnectionToClient connection)
        {
            networkingBusAsSender
                .PopMessage<ClientConnectedMessage>(out var message)
                .Write<ClientConnectedMessage>(message, new object[] { connection })
                .SendImmediately<ClientConnectedMessage>(message);
            
            //ITS EMPTY ANYWAY
            //base.OnServerConnect(connection);
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="connection">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnectionToClient connection)
        {
            networkingBusAsSender
                .PopMessage<ClientDisconnectedMessage>(out var message)
                .Write<ClientDisconnectedMessage>(message, new object[] { connection })
                .SendImmediately<ClientDisconnectedMessage>(message);
            
            //SEND CONFIRMATION FOR BASE LOGIC
            //base.OnServerDisconnect(connection);
        }

        private void OnClientDisconnectionConfirmed(ClientDisconnectionConfirmedMessage message)
        {
            base.OnServerDisconnect(message.Connection);
        }

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="connection">Connection from client.</param>
        public override void OnServerReady(NetworkConnectionToClient connection)
        {
            base.OnServerReady(connection); //TODO: ENSURE IT SHOULD GO FIRST

            networkingBusAsSender
                .PopMessage<ClientReadyToReceiveUpdatesMessage>(out var message)
                .Write<ClientReadyToReceiveUpdatesMessage>(message, new object[] { connection })
                .SendImmediately<ClientReadyToReceiveUpdatesMessage>(message);
        }
        
        /// <summary>
        /// Called on the server when a client adds a new player with NetworkClient.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="connection">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient connection)
        {
            networkingBusAsSender
                .PopMessage<ClientAddedNewPlayerToServerMessage>(out var message)
                .Write<ClientAddedNewPlayerToServerMessage>(message, new object[] { connection })
                .SendImmediately<ClientAddedNewPlayerToServerMessage>(message);
            
            //TODO: ENSURE IT SHOULD NOT CALL THE BASE
        }
        
        /// <summary>
        /// This is invoked when the client is started.
        /// </summary>
        public override void OnStartClient()
        {
            networkingBusAsSender
                .PopMessage<ClientStartedMessage>(out var message)
                .SendImmediately<ClientStartedMessage>(message);
        }

        private void OnStartServerRequest(StartServerRequestMessage message)
        {
            UnityEngine.Debug.Log($"[NetworkingService] STARTING SERVER");
            
            StartServer();
        }
        
        private void OnStopServerRequest(StopServerRequestMessage message)
        {
            UnityEngine.Debug.Log($"[NetworkingService] STOPPING SERVER");
            
            StopServer();
        }
        
        private void OnStartClientRequest(StartClientRequestMessage message)
        {
            UnityEngine.Debug.Log($"[NetworkingService] STARTING CLIENT");
            
            StartClient(message.URI);
        }
        
        private void OnStopClientRequest(StopClientRequestMessage message)
        {
            UnityEngine.Debug.Log($"[NetworkingService] STOPPING CLIENT");
            
            StopClient();
        }
    }
}