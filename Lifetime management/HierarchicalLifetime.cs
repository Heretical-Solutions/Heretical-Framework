using System;

namespace HereticalSolutions.LifetimeManagement
{
    public class HierarchicalLifetime
        : Lifetime
    {
        protected readonly ILifetimeable parentLifetime;

        public HierarchicalLifetime(
            ILifetimeable parentLifetime = null,
            
            ESynchronizationFlags setUpFlags =
                ESynchronizationFlags.SYNC_WITH_PARENT,

            ESynchronizationFlags initializeFlags =
                ESynchronizationFlags.SYNC_WITH_PARENT,

            Func<bool> setUp = null,
            Func<bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null)
            : base (
                setUpFlags,
                initializeFlags,
                setUp,
                initialize,
                cleanup,
                tearDown)
        {
            this.parentLifetime = parentLifetime;
            
            SyncLifetimes(
                this,
                parentLifetime);
            
            CatchUp();
        }
        
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent)
        {
            if (parent == null)
                return;
            
            Action<ILifetimeable> setUpDelegate = null;
            
            Action<ILifetimeable> initializeDelegate = null;
            
            if (target.SetUpFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
                && target is ISetUppable targetSetUppable)
            {
                setUpDelegate = (_) => targetSetUppable.SetUp();

                parent.OnSetUp += setUpDelegate;
            }
            
            if (target.InitializeFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
                && target is IInitializable targetInitializable)
            {
                initializeDelegate = (_) => targetInitializable.Initialize();
                
                parent.OnInitialized += initializeDelegate;
            }
            
            Action<ILifetimeable> cleanUpDelegate = null;
            
            if (target is ICleanuppable targetCleanuppable)
            {
                cleanUpDelegate = (_) => targetCleanuppable.Cleanup();
                
                parent.OnCleanedUp += cleanUpDelegate;
            }

            Action<ILifetimeable> tearDownDelegate = null;

            if (target is ITearDownable targetTearDownable)
            {
                tearDownDelegate = (_) =>
                {
                    parent.OnSetUp -= setUpDelegate;
                    parent.OnInitialized -= initializeDelegate;
                    parent.OnCleanedUp -= cleanUpDelegate;
                    parent.OnTornDown -= tearDownDelegate;

                    targetTearDownable.TearDown();
                };

                parent.OnTornDown += tearDownDelegate;
            }
        }

        protected void CatchUp()
        {
            if (parentLifetime == null)
                return;
            
            if (setUpFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
                && !isSetUp
                && parentLifetime.IsSetUp)
            {
                SetUp();
            }
            
            if (initializeFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
                && !isInitialized
                && parentLifetime.IsInitialized)
            {
                Initialize();
            }
        }
    }
}