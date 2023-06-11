using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Services.Factories
{
    public static partial class ServicesFactory
    {
        public static void InitializeSynchronizationService(
            SynchronizationService service,
            TimeManager timeManager)
        {
            ISynchronizable synchronizableFixedUpdate = TimeFactory.BuildSynchronizationContext(
                ESynchronizationPoints.FIXED_UPDATE.ToString(),
                true,
                false);
            
            ISynchronizable synchronizableUpdate = TimeFactory.BuildSynchronizationContext(
                ESynchronizationPoints.UPDATE.ToString(),
                true,
                true);
            
            ISynchronizable synchronizableLateUpdate = TimeFactory.BuildSynchronizationContext(
                ESynchronizationPoints.LATE_UPDATE.ToString(),
                true,
                true);
            
            ISynchronizable synchronizableUIUpdate = TimeFactory.BuildSynchronizationContext(
                ESynchronizationPoints.UI_UPDATE.ToString(),
                true,
                true);
            
            service.Initialize(
                timeManager,
                synchronizableFixedUpdate,
                synchronizableUpdate,
                synchronizableLateUpdate,
                synchronizableUIUpdate);
        }
    }
}