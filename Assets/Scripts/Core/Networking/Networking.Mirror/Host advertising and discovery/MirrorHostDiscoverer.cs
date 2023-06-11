using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;
using HereticalSolutions.Delegates.Subscriptions;
using HereticalSolutions.Messaging;
using HereticalSolutions.Time;

using Mirror;

using UnityEngine;

namespace HereticalSolutions.Networking.Mirror
{
    public class MirrorHostDiscoverer<TRequest, TResponse>
        : IHostDiscoverer
        where TRequest : NetworkMessage
        where TResponse : NetworkMessage
    {
        #region Message bus

        //private readonly INonAllocMessageSender networkingBusAsSender;
        
        private readonly INonAllocMessageReceiver networkingBusAsReceiver;

        #endregion

        #region Subscriptions

        private readonly ISubscription startHostDiscoveryRequestSubscription;
        
        private readonly ISubscription stopHostDiscoveryRequestSubscription;

        #endregion
        
        #region Timer
        
        private readonly ISynchronizationProvider updater;

        private readonly IRuntimeTimer discoveryRequestTimer;
        
        private readonly ITickable timerAsTickable;

        private readonly ISubscription updaterSubscription;

        private readonly SubscriptionSingleArgGeneric<IRuntimeTimer> discoveryRequestSubscription;

        #endregion
        
        private readonly MirrorHostAdvertisingAndDiscoverySettings settings;

        private readonly NetworkingContext context;

        private UdpClient clientUdpClient;

        private readonly IRequestMaker<TRequest> requestMaker;
        
        private readonly IResponseProcessor<TResponse> responseProcessor;

        private bool discovering;

        public MirrorHostDiscoverer(
            //INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            
            MirrorHostAdvertisingAndDiscoverySettings settings,
            NetworkingContext context,
            
            IRequestMaker<TRequest> requestMaker,
            IResponseProcessor<TResponse> responseProcessor,
            
            ISynchronizationProvider updater,
            IRuntimeTimer discoveryRequestTimer,
            ITickable timerAsTickable)
        {
            #region Message bus

            //this.networkingBusAsSender = networkingBusAsSender;

            this.networkingBusAsReceiver = networkingBusAsReceiver;
            
            #endregion

            #region Subscriptions

            startHostDiscoveryRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StartHostDiscoveryRequestMessage>(OnStartHostDiscoveryRequest);
            
            stopHostDiscoveryRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StopHostDiscoveryRequestMessage>(OnStopHostDiscoveryRequest);

            networkingBusAsReceiver.SubscribeTo<StartHostDiscoveryRequestMessage>(startHostDiscoveryRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StopHostDiscoveryRequestMessage>(stopHostDiscoveryRequestSubscription);
            
            #endregion
            
            #region Timer

            this.updater = updater;
            
            this.discoveryRequestTimer = discoveryRequestTimer;

            this.timerAsTickable = timerAsTickable;

            
            //moved to factory
            //broadcastDiscoveryRequestTimer.Reset(settings.ActiveDiscoveryInterval);
            
            discoveryRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(BroadcastDiscoveryRequest);
            
            discoveryRequestTimer.OnFinish.Subscribe(discoveryRequestSubscription);
            
            updaterSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(this.timerAsTickable.Tick);
            
            updater.Subscribe(updaterSubscription);
            
            #endregion
            
            this.settings = settings;

            this.context = context;

            this.requestMaker = requestMaker;
            
            this.responseProcessor = responseProcessor;

            clientUdpClient = null;

            discovering = false;
        }

        public void Deinitialize()
        {
            if (discoveryRequestSubscription.Active)
                discoveryRequestTimer.OnFinish.Unsubscribe(discoveryRequestSubscription);

            if (updaterSubscription.Active)
                updater.Unsubscribe(updaterSubscription);
            
            if (startHostDiscoveryRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StartHostDiscoveryRequestMessage>(startHostDiscoveryRequestSubscription);
            
            if (stopHostDiscoveryRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StopHostDiscoveryRequestMessage>(stopHostDiscoveryRequestSubscription);
        }

        public void Shutdown()
        {
            Debug.Log($"[MirrorHostDiscoverer] SHUTTING DOWN");
            
            if (clientUdpClient != null)
            {
                try
                {
                    clientUdpClient.Close();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                clientUdpClient = null;
            }

            if (updaterSubscription.Active)
                discoveryRequestTimer.Abort();

            discovering = false;
        }

        #region IHostDiscoverer

        public bool Discovering
        {
            get => discovering;
        }

        /// <summary>
        /// Start Active Discovery
        /// </summary>
        public void StartDiscovery()
        {
            //StopDiscovery();
            
            if (discovering)
                return;

            Debug.Log($"[MirrorHostDiscoverer] STARTING DISCOVERING");
            
            try
            {
                // Setup port
                clientUdpClient = new UdpClient(0)
                {
                    EnableBroadcast = true,
                    MulticastLoopback = false
                };
            }
            catch (Exception)
            {
                // Free the port if we took it
                //Debug.LogError("NetworkDiscoveryBase StartDiscovery Exception");
                Shutdown();
                
                throw;
            }

            var listenTask = ClientListenAsync();

            if (settings.EnableActiveDiscovery)
            {
                discoveryRequestTimer.Start();
            }

            discovering = true;
        }

        /// <summary>
        /// Stop Active Discovery
        /// </summary>
        public void StopDiscovery()
        {
            if (!discovering)
                return;
            
            Debug.Log($"[MirrorHostDiscoverer] STOPPING DISCOVERING");
            
            //Debug.Log("NetworkDiscoveryBase StopDiscovery");
            Shutdown();
        }

        /// <summary>
        /// Awaits for server response
        /// </summary>
        /// <returns>ClientListenAsync Task</returns>
        private async Task ClientListenAsync()
        {
            //Debug.Log("[MirrorHostDiscoverer] ClientListenAsync");
            
            // while clientUpdClient to fix: 
            // https://github.com/vis2k/Mirror/pull/2908
            //
            // If, you cancel discovery the clientUdpClient is set to null.
            // However, nothing cancels ClientListenAsync. If we change the if(true)
            // to check if the client is null. You can properly cancel the discovery, 
            // and kill the listen thread.
            //
            // Prior to this fix, if you cancel the discovery search. It crashes the 
            // thread, and is super noisy in the output. As well as causes issues on 
            // the quest.
            while (clientUdpClient != null)
            {
                try
                {
                    await ReceiveGameBroadcastAsync(clientUdpClient);
                }
                catch (ObjectDisposedException)
                {
                    // socket was closed, no problem
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void BroadcastDiscoveryRequest(IRuntimeTimer timer)
        {
            BroadcastDiscoveryRequest();
        }

        /// <summary>
        /// Sends discovery request from client
        /// </summary>
        private void BroadcastDiscoveryRequest()
        {
            //Debug.Log("[MirrorHostDiscoverer] BroadcastDiscoveryRequest");
            
            if (clientUdpClient == null)
                return;

            if (NetworkClient.isConnected)
            {
                StopDiscovery();
                
                return;
            }

            IPEndPoint endPoint = new IPEndPoint(
                IPAddress.Broadcast,
                settings.ServerBroadcastListenPort);
            
            if (!string.IsNullOrWhiteSpace(settings.BroadcastAddress))
            {
                try
                {
                    endPoint = new IPEndPoint(
                        IPAddress.Parse(settings.BroadcastAddress),
                        settings.ServerBroadcastListenPort);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            using (NetworkWriterPooled writer = NetworkWriterPool.Get())
            {
                writer.WriteLong(settings.SecretHandshake);

                try
                {
                    var request = requestMaker.MakeRequest(context);

                    writer.Write(request);

                    ArraySegment<byte> data = writer.ToArraySegment();

                    //Debug.Log("[MirrorHostDiscoverer] SENDING BROADCAST PACKET");
                    
                    clientUdpClient.SendAsync(
                        data.Array,
                        data.Count,
                        endPoint);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        async Task ReceiveGameBroadcastAsync(UdpClient udpClient)
        {
            // only proceed if there is available data in network buffer, or otherwise Receive() will block
            // average time for UdpClient.Available : 10 us

            UdpReceiveResult udpReceiveResult = await udpClient.ReceiveAsync();
            
            using (NetworkReaderPooled networkReader = NetworkReaderPool.Get(udpReceiveResult.Buffer))
            {
                var handshake = networkReader.ReadLong(); 
             
                Debug.Log($"[MirrorHostAdvertiser] RECEIVED MESSAGE. HANDSHAKE: {handshake}");
                
                if (handshake != settings.SecretHandshake)
                    return;

                TResponse response = networkReader.Read<TResponse>();

                if (response == null)
                    Debug.LogError($"[MirrorHostDiscoverer] COULD NOT READ RESPONSE BY TYPE {typeof(TResponse)}");
                
                responseProcessor.ProcessResponse(
                    response,
                    udpReceiveResult.RemoteEndPoint,
                    context);
            }
        }

        #endregion
        
        private void OnStartHostDiscoveryRequest(StartHostDiscoveryRequestMessage message)
        {
            StartDiscovery();
        }
        
        private void OnStopHostDiscoveryRequest(StopHostDiscoveryRequestMessage message)
        {
            StopDiscovery();
        }
    }
}