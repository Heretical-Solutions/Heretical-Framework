using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Bags.Factories;
using HereticalSolutions.Bags.NonAlloc.Factories;

using HereticalSolutions.Synchronization.Time.TimeUpdaters;
using HereticalSolutions.Synchronization.Time.TimeUpdaters.Factories;
using HereticalSolutions.Synchronization.Time.Timers.FloatDelta.Factories;
using HereticalSolutions.Synchronization.Time.Timers.TickCollection.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

namespace HereticalSolutions.Synchronization.Unity
{
    public class SynchronizationManager
        : MonoBehaviour,
          ISynchronizationManager,
          ICleanuppable,
          IDisposable
    {
        private ITickUpdater fixedUpdateTick;
        private ITimeUpdater fixedUpdateTimer;

        private ITickUpdater updateTick;
        private ITimeUpdater updateTimer;

        private ITickUpdater lateUpdateTick;
        private ITimeUpdater lateUpdateTimer;

        //TODO: inject it all with a container instead of doing poor man's DI
		//void Awake()
		//{
        //    ILoggerResolver loggerResolver = null;
        //
        //    DelegateWrapperFactory delegateWrapperFactory =
        //        new DelegateWrapperFactory(
        //            loggerResolver);
        //
        //    NonAllocSubscriptionFactory nonAllocSubscriptionFactory =
        //        new NonAllocSubscriptionFactory(
        //            delegateWrapperFactory,
        //            loggerResolver);
        //
        //    ConfigurableStackPoolFactory configurableStackPoolFactory =
        //        new ConfigurableStackPoolFactory(
        //            loggerResolver);
        //
        //    NonAllocLinkedListBagFactory nonAllocLinkedListBagFactory =
        //        new NonAllocLinkedListBagFactory(
        //            configurableStackPoolFactory);
        //
        //    NonAllocPingerFactory nonAllocPingerFactory =
        //        new NonAllocPingerFactory(
        //            configurableStackPoolFactory,
        //            nonAllocLinkedListBagFactory,
        //            loggerResolver);
        //
		//	NonAllocBroadcasterFactory nonAllocBroadcasterFactory =
        //        new NonAllocBroadcasterFactory(
        //            configurableStackPoolFactory,
        //            nonAllocLinkedListBagFactory,
        //            loggerResolver);
        //
        //    CollectionFactory collectionFactory =
        //        new CollectionFactory();
        //
        //    RepositoryFactory repositoryFactory =
        //        new RepositoryFactory(
        //            collectionFactory,
        //            loggerResolver);
        //
        //    FloatTimerFactory floatTimerFactory =
        //        new FloatTimerFactory(
        //            nonAllocBroadcasterFactory,
        //            repositoryFactory,
        //            nonAllocSubscriptionFactory);
        //
		//	TickTimerFactory tickTimerFactory =
        //        new TickTimerFactory(
        //            nonAllocBroadcasterFactory,
        //            repositoryFactory,
        //            nonAllocSubscriptionFactory);
        //
        //    TimeUpdaterFactory timeUpdaterFactory = new TimeUpdaterFactory(
        //        nonAllocSubscriptionFactory,
        //        nonAllocPingerFactory,
        //        nonAllocBroadcasterFactory,
        //        floatTimerFactory,
        //        tickTimerFactory);
        //
        //    //For togglable, scalable time updaters and the ones with custom fixed delta,
        //    //Create the new ones and synchronize them with the ones below. They can act
        //    //as hierarchical synchronization points, like time updates for game logic,
        //    //time updaters for renderting, etc. They can be slowed down or toggled
        //    //individually without affecting other updaters
        //
        //    fixedUpdateTick = timeUpdaterFactory
        //        .BuildTickUpdater(
        //            "Fixed update tick",
        //            togglable: false,
        //            active: true);
        //
        //    fixedUpdateTimer = timeUpdaterFactory
        //        .BuildTimeUpdater(
        //            "Fixed update timer",
        //            togglable: false,
        //            active: true,
        //            scalable: false,
        //            hasFixedDelta: false,
        //            canHaveNegativeDelta: false);
        //
        //    updateTick = timeUpdaterFactory
        //        .BuildTickUpdater(
        //            "Update tick",
        //            togglable: false,
        //            active: true);
        //
        //    updateTimer = timeUpdaterFactory
        //        .BuildTimeUpdater(
        //            "Update timer",
        //            togglable: false,
        //            active: true,
        //            scalable: false,
        //            hasFixedDelta: false,
        //            canHaveNegativeDelta: false);
        //
        //    lateUpdateTick = timeUpdaterFactory
        //        .BuildTickUpdater(
        //            "Late update tick",
        //            togglable: false,
        //            active: true);
        //
        //    lateUpdateTimer = timeUpdaterFactory
        //        .BuildTimeUpdater(
        //            "Late update timer",
        //            togglable: false,
        //            active: true,
        //            scalable: false,
        //            hasFixedDelta: false,
        //            canHaveNegativeDelta: false);
        //}

        public void Initialize(
            TimeUpdaterFactory timeUpdaterFactory)
        {
            //For togglable, scalable time updaters and the ones with custom fixed delta,
            //Create the new ones and synchronize them with the ones below. They can act
            //as hierarchical synchronization points, like time updates for game logic,
            //time updaters for renderting, etc. They can be slowed down or toggled
            //individually without affecting other updaters
        
            fixedUpdateTick = timeUpdaterFactory
                .BuildTickUpdater(
                    "Fixed update tick",
                    togglable: false,
                    active: true);
        
            fixedUpdateTimer = timeUpdaterFactory
                .BuildTimeUpdater(
                    "Fixed update timer",
                    togglable: false,
                    active: true,
                    scalable: false,
                    hasFixedDelta: false,
                    canHaveNegativeDelta: false);
        
            updateTick = timeUpdaterFactory
                .BuildTickUpdater(
                    "Update tick",
                    togglable: false,
                    active: true);
        
            updateTimer = timeUpdaterFactory
                .BuildTimeUpdater(
                    "Update timer",
                    togglable: false,
                    active: true,
                    scalable: false,
                    hasFixedDelta: false,
                    canHaveNegativeDelta: false);
        
            lateUpdateTick = timeUpdaterFactory
                .BuildTickUpdater(
                    "Late update tick",
                    togglable: false,
                    active: true);
        
            lateUpdateTimer = timeUpdaterFactory
                .BuildTimeUpdater(
                    "Late update timer",
                    togglable: false,
                    active: true,
                    scalable: false,
                    hasFixedDelta: false,
                    canHaveNegativeDelta: false);
        }
    
        #region ISynchronizationManager

        public ITickUpdater FixedUpdateTick => fixedUpdateTick;
        public ITimeUpdater FixedUpdateTimer => fixedUpdateTimer;

        public ITickUpdater UpdateTick => updateTick;
        public ITimeUpdater UpdateTimer => updateTimer;

        public ITickUpdater LateUpdateTick => lateUpdateTick;
        public ITimeUpdater LateUpdateTimer => lateUpdateTimer;

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (fixedUpdateTick is ICleanuppable)
                (fixedUpdateTick as ICleanuppable).Cleanup();

            if (fixedUpdateTimer is ICleanuppable)
                (fixedUpdateTimer as ICleanuppable).Cleanup();

            if (updateTick is ICleanuppable)
                (updateTick as ICleanuppable).Cleanup();

            if (updateTimer is ICleanuppable)
                (updateTimer as ICleanuppable).Cleanup();

            if (lateUpdateTick is ICleanuppable)
                (lateUpdateTick as ICleanuppable).Cleanup();

            if (lateUpdateTimer is ICleanuppable)
                (lateUpdateTimer as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (fixedUpdateTick is IDisposable)
                (fixedUpdateTick as IDisposable).Dispose();

            if (fixedUpdateTimer is IDisposable)
                (fixedUpdateTimer as IDisposable).Dispose();

            if (updateTick is IDisposable)
                (updateTick as IDisposable).Dispose();

            if (updateTimer is IDisposable)
                (updateTimer as IDisposable).Dispose();

            if (lateUpdateTick is IDisposable)
                (lateUpdateTick as IDisposable).Dispose();

            if (lateUpdateTimer is IDisposable)
                (lateUpdateTimer as IDisposable).Dispose();
        }

        #endregion

        void FixedUpdate()
        {
            fixedUpdateTick.Synchronize();

            fixedUpdateTimer.Synchronize(
                UnityEngine.Time.fixedUnscaledDeltaTime);
        }

        void Update()
        {
            updateTick.Synchronize();

            updateTimer.Synchronize(
                UnityEngine.Time.unscaledDeltaTime);
        }

        void LateUpdate()
        {
            lateUpdateTick.Synchronize();

            lateUpdateTimer.Synchronize(
                UnityEngine.Time.unscaledDeltaTime);
        }
	}
}