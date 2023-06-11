using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            string id = "Anonymous Timer",
            float defaultDuration = 0f)
        {
            return new SetRuntimeTimerCallback<T>(id, defaultDuration);
        }
        
        #endregion
    }
}