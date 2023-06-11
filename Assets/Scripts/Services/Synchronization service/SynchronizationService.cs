using HereticalSolutions.Time;

using UnityEngine;

namespace HereticalSolutions.Services
{
    public class SynchronizationService : MonoBehaviour
    {
        private TimeManager timeManager;

        private ISynchronizable synchronizableFixedUpdate;
        
        private ISynchronizable synchronizableUpdate;
        
        private ISynchronizable synchronizableLateUpdate;
        
        private ISynchronizable synchronizableUIUpdate;
        
        public void Initialize(
            TimeManager timeManager,
            ISynchronizable synchronizableFixedUpdate,
            ISynchronizable synchronizableUpdate,
            ISynchronizable synchronizableLateUpdate,
            ISynchronizable synchronizableUIUpdate)
        {
            this.timeManager = timeManager;
            
            this.synchronizableFixedUpdate = synchronizableFixedUpdate;
            
            timeManager.AddSynchronizable(
                ESynchronizationPoints.FIXED_UPDATE.ToString(),
                this.synchronizableFixedUpdate);
            
            
            this.synchronizableUpdate = synchronizableUpdate;
            
            timeManager.AddSynchronizable(
                ESynchronizationPoints.UPDATE.ToString(),
                this.synchronizableUpdate);
            
            
            this.synchronizableLateUpdate = synchronizableLateUpdate;
            
            timeManager.AddSynchronizable(
                ESynchronizationPoints.LATE_UPDATE.ToString(),
                this.synchronizableLateUpdate);
            
            
            this.synchronizableUIUpdate = synchronizableUIUpdate;
            
            timeManager.AddSynchronizable(
                ESynchronizationPoints.UI_UPDATE.ToString(),
                this.synchronizableUIUpdate);
        }

        private void FixedUpdate()
        {
            synchronizableFixedUpdate?.Synchronize(UnityEngine.Time.fixedDeltaTime);
        }

        private void Update()
        {
            synchronizableUpdate?.Synchronize(UnityEngine.Time.deltaTime);
        }

        private void LateUpdate()
        {
            synchronizableLateUpdate?.Synchronize(UnityEngine.Time.deltaTime);
            
            synchronizableUIUpdate?.Synchronize(UnityEngine.Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (timeManager == null)
                return;
            
            if (synchronizableFixedUpdate != null)
                timeManager.RemoveSynchronizable(
                    ESynchronizationPoints.FIXED_UPDATE.ToString());
            
            if (synchronizableUpdate != null)
                timeManager.RemoveSynchronizable(
                    ESynchronizationPoints.UPDATE.ToString());
            
            if (synchronizableLateUpdate != null)
                timeManager.RemoveSynchronizable(
                    ESynchronizationPoints.LATE_UPDATE.ToString());
            
            if (synchronizableUIUpdate != null)
                timeManager.RemoveSynchronizable(
                    ESynchronizationPoints.UI_UPDATE.ToString());
        }
    }
}