using System;

namespace HereticalSolutions.LifetimeManagement.Factories
{
    public static class LifetimeFactory
    {
        public static Lifetime BuildLifetime(
            object target = null,
            
            ESynchronizationFlags setUpFlags = 
                ESynchronizationFlags.SYNC_WITH_PARENT,
            
            ESynchronizationFlags initializeFlags = 
                ESynchronizationFlags.SYNC_WITH_PARENT,
			
            Func<bool> setUp = null,
            Func<bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null)
        {
            if (setUp == null
                && target is ISetUppable targetSetUppable)
            {
                setUp = targetSetUppable.SetUp;
            }
            
            if (initialize == null
                && target is IInitializable targetInitializable)
            {
                initialize = targetInitializable.Initialize;
            }
            
            if (cleanup == null
                && target is ICleanuppable targetCleanuppable)
            {
                cleanup = targetCleanuppable.Cleanup;
            }
            
            if (tearDown == null
                && target is ITearDownable targetTearDownable)
            {
                tearDown = targetTearDownable.TearDown;
            }

            var result = new Lifetime(
                setUpFlags,
                initializeFlags,
                setUp,
                initialize,
                cleanup,
                tearDown);
            
            if (target is IContainsLifetime targetContainsLifetime)
            {
                targetContainsLifetime.Lifetime = result;
            }
            
            return result;
        }

        public static HierarchicalLifetime BuildHierarchicalLifetime(
            ILifetimeable parent = null,

            object target = null,

            ESynchronizationFlags setUpFlags =
                ESynchronizationFlags.SYNC_WITH_PARENT,

            ESynchronizationFlags initializeFlags =
                ESynchronizationFlags.SYNC_WITH_PARENT,

            Func<bool> setUp = null,
            Func<bool> initialize = null,
            Action cleanup = null,
            Action tearDown = null)
        {
            if (setUp == null
                && target is ISetUppable targetSetUppable)
            {
                setUp = targetSetUppable.SetUp;
            }

            if (initialize == null
                && target is IInitializable targetInitializable)
            {
                initialize = targetInitializable.Initialize;
            }

            if (cleanup == null
                && target is ICleanuppable targetCleanuppable)
            {
                cleanup = targetCleanuppable.Cleanup;
            }

            if (tearDown == null
                && target is ITearDownable targetTearDownable)
            {
                tearDown = targetTearDownable.TearDown;
            }

            var result = new HierarchicalLifetime(
                parent,
                setUpFlags,
                initializeFlags,
                setUp,
                initialize,
                cleanup,
                tearDown);

            if (target is IContainsLifetime targetContainsLifetime)
            {
                targetContainsLifetime.Lifetime = result;
            }

            return result;
        }
    }
}