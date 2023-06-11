using HereticalSolutions.Networking;

using UnityEngine;

namespace HereticalSolutions.Services
{
    public class HostAdvertisingService
        : MonoBehaviour,
          IHostAdvertisingService
    {
        private IHostAdvertiser hostAdvertiser;

        public void Initialize(IHostAdvertiser hostAdvertiser)
        {
            this.hostAdvertiser = hostAdvertiser;
        }

        public void StartAdvertising()
        {
            hostAdvertiser?.StartAdvertising();
        }

        public void StopAdvertising()
        {
            hostAdvertiser?.StopAdvertising();
        }

        // Ensure the ports are cleared no matter when Game/Unity UI exits
        void OnApplicationQuit()
        {
            hostAdvertiser?.Shutdown();
        }

        void OnDisable()
        {
            hostAdvertiser?.Shutdown();
        }

        void OnDestroy()
        {
            hostAdvertiser?.Shutdown();
        }
    }
}