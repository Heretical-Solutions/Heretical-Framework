using System;

namespace HereticalSolutions.LifetimeManagement
{
    [Flags]
    public enum ESynchronizationFlags
    {
        NONE = 0,
        
        SYNC_WITH_PARENT = 1 << 0
    }
}