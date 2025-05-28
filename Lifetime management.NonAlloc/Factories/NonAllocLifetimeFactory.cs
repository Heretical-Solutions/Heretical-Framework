using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.LifetimeManagement.NonAlloc.Factories
{
    public class NonAllocLifetimeFactory
    {
        private readonly NonAllocBroadcasterFactory nonAllocBroadcasterFactory;

        public NonAllocLifetimeFactory(
            NonAllocBroadcasterFactory nonAllocBroadcasterFactory)
        {
            this.nonAllocBroadcasterFactory = nonAllocBroadcasterFactory;
        }

        public NonAllocLifetime BuildNonAllocLifetime(
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

            var result = new NonAllocLifetime(
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),

                setUpFlags,
                initializeFlags,
                
                setUp,
                initialize,
                cleanup,
                tearDown);

            if (target is IContainsNonAllocLifetime targetContainsLifetime)
            {
                targetContainsLifetime.Lifetime = result;
            }

            return result;
        }
        
        public NonAllocHierarchicalLifetime BuildNonAllocHierarchicalLifetime(
            IHierarchyNode<INonAllocLifetimeable> hierarchyNode,
            IPool<List<IReadOnlyHierarchyNode<INonAllocLifetimeable>>> bufferPool,

            INonAllocLifetimeable parent = null,
        
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

            var result = new NonAllocHierarchicalLifetime(
                hierarchyNode,
                bufferPool,

                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),
                nonAllocBroadcasterFactory.
                    BuildNonAllocBroadcasterGeneric<INonAllocLifetimeable>(),

                setUpFlags,
                initializeFlags,

                parent,

                setUp,
                initialize,
                cleanup,
                tearDown);

            if (target is IContainsNonAllocLifetime targetContainsLifetime)
            {
                targetContainsLifetime.Lifetime = result;
            }

            return result;
        }
    }
}