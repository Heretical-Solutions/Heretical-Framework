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
    public class MirrorHostAdvertiser<TRequest, TResponse>
        : IHostAdvertiser
        where TRequest : NetworkMessage
        where TResponse : NetworkMessage
    {
        #region Message bus

        //private readonly INonAllocMessageSender networkingBusAsSender;
        
        private readonly INonAllocMessageReceiver networkingBusAsReceiver;

        #endregion

        #region Subscriptions

        private readonly ISubscription startHostAdvertisingRequestSubscription;
        
        private readonly ISubscription stopHostAdvertisingRequestSubscription;

        #endregion
        
        #region Timer
        
        private readonly ISynchronizationProvider synchronizationProvider;

        private readonly IRuntimeTimer broadcastDiscoveryRequestTimer;
        
        private readonly ITickable broadcastDiscoveryRequestTimerAsTickable;

        private readonly ISubscription timerSubscription;

        private readonly SubscriptionSingleArgGeneric<IRuntimeTimer> broadcastDiscoveryRequestSubscription;

        #endregion
        
        private readonly MirrorHostAdvertisingAndDiscoverySettings settings;

        private readonly NetworkingContext context;

        private UdpClient serverUdpClient;

        private readonly IRequestProcessor<TRequest, TResponse> requestProcessor;

        private bool advertising;

        public MirrorHostAdvertiser(
            //INonAllocMessageSender networkingBusAsSender,
            INonAllocMessageReceiver networkingBusAsReceiver,
            
            MirrorHostAdvertisingAndDiscoverySettings settings,
            NetworkingContext context,
            
            IRequestProcessor<TRequest, TResponse> requestProcessor)
        {
            #region Message bus

            //this.networkingBusAsSender = networkingBusAsSender;

            this.networkingBusAsReceiver = networkingBusAsReceiver;
            
            #endregion

            #region Subscriptions

            startHostAdvertisingRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StartHostAdvertisingRequestMessage>(OnStartHostAdvertisingRequest);
            
            stopHostAdvertisingRequestSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<StopHostAdvertisingRequestMessage>(OnStopHostAdvertisingRequest);

            networkingBusAsReceiver.SubscribeTo<StartHostAdvertisingRequestMessage>(startHostAdvertisingRequestSubscription);
            
            networkingBusAsReceiver.SubscribeTo<StopHostAdvertisingRequestMessage>(stopHostAdvertisingRequestSubscription);
            
            #endregion
            
            this.settings = settings;

            this.context = context;

            this.requestProcessor = requestProcessor;

            serverUdpClient = null;

            advertising = false;
        }

        public void Deinitialize()
        {
            if (startHostAdvertisingRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StartHostAdvertisingRequestMessage>(startHostAdvertisingRequestSubscription);
            
            if (stopHostAdvertisingRequestSubscription.Active)
                networkingBusAsReceiver.UnsubscribeFrom<StopHostAdvertisingRequestMessage>(stopHostAdvertisingRequestSubscription);
        }
        
        public void Shutdown()
        {
            Debug.Log($"[MirrorHostAdvertiser] SHUTTING DOWN");
            
            EndpMulticastLock();
            
            if (serverUdpClient != null)
            {
                try
                {
                    serverUdpClient.Close();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                serverUdpClient = null;
            }

            advertising = false;
        }
        
        #region IHostAdvertiser

        public bool Advertising
        {
            get => advertising;
        }

        /// <summary>
        /// Advertise this server in the local network
        /// </summary>
        public void StartAdvertising()
        {
            if (advertising)
                return;
            
            Debug.Log($"[MirrorHostAdvertiser] STARTING ADVERTISING");
            
            // Setup port -- may throw exception
            serverUdpClient = new UdpClient(settings.ServerBroadcastListenPort)
            {
                EnableBroadcast = true,
                MulticastLoopback = false
            };

            // listen for client pings
            var listenTask = ServerListenAsync();

            advertising = true;
        }
        
        public void StopAdvertising()
        {
            if (!advertising)
                return;

            Debug.Log($"[MirrorHostAdvertiser] STOPPING ADVERTISING");

            advertising = false;
            
            Shutdown();
        }

        private async Task ServerListenAsync()
        {
            BeginMulticastLock();
            
            while (true)
            {
                try
                {
                    await ReceiveRequestAsync(serverUdpClient);
                }
                catch (ObjectDisposedException)
                {
                    // socket has been closed
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private async Task ReceiveRequestAsync(UdpClient udpClient)
        {
            // only proceed if there is available data in network buffer, or otherwise Receive() will block
            // average time for UdpClient.Available : 10 us

            UdpReceiveResult udpReceiveResult = await udpClient.ReceiveAsync();

            using (NetworkReaderPooled networkReader = NetworkReaderPool.Get(udpReceiveResult.Buffer))
            {
                long handshake = networkReader.ReadLong();
                
                Debug.Log($"[MirrorHostAdvertiser] RECEIVED MESSAGE. HANDSHAKE: {handshake}");
                
                if (handshake != settings.SecretHandshake)
                {
                    // message is not for us
                    throw new ProtocolViolationException("Invalid handshake");
                }

                TRequest request = networkReader.Read<TRequest>();

                ProcessClientRequest(request, udpReceiveResult.RemoteEndPoint);
            }
        }

        /// <summary>
        /// Reply to the client to inform it of this server
        /// </summary>
        /// <remarks>
        /// Override if you wish to ignore server requests based on
        /// custom criteria such as language, full server game mode or difficulty
        /// </remarks>
        /// <param name="request">Request coming from client</param>
        /// <param name="endpoint">Address of the client that sent the request</param>
        private void ProcessClientRequest(TRequest request, IPEndPoint endpoint)
        {
            if (!requestProcessor.ProcessRequest(
                    request,
                    endpoint,
                    context,
                    out TResponse response))
                return;
            
            using (NetworkWriterPooled writer = NetworkWriterPool.Get())
            {
                try
                {
                    writer.WriteLong(settings.SecretHandshake);

                    writer.Write(response);

                    ArraySegment<byte> data = writer.ToArraySegment();
                    
                    // signature matches
                    // send response
                    serverUdpClient.Send(
                        data.Array,
                        data.Count,
                        endpoint);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        // Android Multicast fix: https://github.com/vis2k/Mirror/pull/2887
#if UNITY_ANDROID
        AndroidJavaObject multicastLock;
        
        bool hasMulticastLock;
#endif

        private void BeginMulticastLock()
		{
#if UNITY_ANDROID
            if (hasMulticastLock)
                return;
                
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        multicastLock = wifiManager.Call<AndroidJavaObject>("createMulticastLock", "lock");
                        multicastLock.Call("acquire");
                        hasMulticastLock = true;
                    }
                }
			}
#endif
        }

        private void EndpMulticastLock()
        {
#if UNITY_ANDROID
            if (!hasMulticastLock) return;
            
            multicastLock?.Call("release");
            hasMulticastLock = false;
#endif
        }

#endregion

        private void OnStartHostAdvertisingRequest(StartHostAdvertisingRequestMessage message)
        {
            StartAdvertising();
        }
                
        private void OnStopHostAdvertisingRequest(StopHostAdvertisingRequestMessage message)
        {
            StopAdvertising();
        }
    }
}