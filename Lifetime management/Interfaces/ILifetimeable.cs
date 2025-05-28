using System;

namespace HereticalSolutions.LifetimeManagement
{
    public interface ILifetimeable
    {
        #region Set up

        bool IsSetUp { get; }

        Action<ILifetimeable> OnSetUp { get; set; }
        
        ESynchronizationFlags SetUpFlags { get; }
        
        #endregion

        #region Initialize

        bool IsInitialized { get; }

        Action<ILifetimeable> OnInitialized { get; set; }
        
        ESynchronizationFlags InitializeFlags { get; }

        #endregion

        #region Cleanup

        Action<ILifetimeable> OnCleanedUp { get; set; }

        #endregion

        #region Tear down

        Action<ILifetimeable> OnTornDown { get; set; }

        #endregion
    }
}