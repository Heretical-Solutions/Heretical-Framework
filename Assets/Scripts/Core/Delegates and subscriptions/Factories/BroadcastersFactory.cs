using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Broadcaster multiple args

        public static BroadcasterMultipleArgs BuildBroadcasterMultipleArgs()
        {
            return new BroadcasterMultipleArgs(
                BuildBroadcasterGeneric<object[]>());
        }

        #endregion
        
        #region Broadcaster with repository

        public static BroadcasterWithRepository BuildBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository)
        {
            return BuildBroadcasterWithRepository(RepositoriesFactory.BuildDictionaryObjectRepository(broadcastersRepository));
        }
        
        public static BroadcasterWithRepository BuildBroadcasterWithRepository(IReadOnlyObjectRepository repository)
        {
            return new BroadcasterWithRepository(repository);
        }

        #endregion
        
        #region Broadcaster generic

        public static BroadcasterGeneric<T> BuildBroadcasterGeneric<T>()
        {
            return new BroadcasterGeneric<T>();
        }

        #endregion
        
        #region Non alloc broadcaster multiple args
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs()
        {
            Func<IInvokableMultipleArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableMultipleArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableMultipleArgs>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });

            return BuildNonAllocBroadcasterMultipleArgs(subscriptionsPool);
        }

        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional)
        {
            Func<IInvokableMultipleArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableMultipleArgs>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableMultipleArgs>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional);

            return BuildNonAllocBroadcasterMultipleArgs(subscriptionsPool);
        }
        
        public static NonAllocBroadcasterMultipleArgs BuildNonAllocBroadcasterMultipleArgs(
            INonAllocDecoratedPool<IInvokableMultipleArgs> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<IInvokableMultipleArgs>>)subscriptionsPool).Contents;
			
            return new NonAllocBroadcasterMultipleArgs(
                subscriptionsPool,
                contents);
        }
        
        #endregion
        
        #region Non alloc broadcaster with repository
        
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IRepository<Type, object> broadcastersRepository)
        {
            return BuildNonAllocBroadcasterWithRepository(RepositoriesFactory.BuildDictionaryObjectRepository(broadcastersRepository));
        }
        
        public static NonAllocBroadcasterWithRepository BuildNonAllocBroadcasterWithRepository(
            IReadOnlyObjectRepository repository)
        {
            return new NonAllocBroadcasterWithRepository(repository);
        }
        
        #endregion
        
        #region Non alloc broadcaster generic
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>()
        {
            Func<IInvokableSingleArgGeneric<T>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableSingleArgGeneric<T>>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableSingleArgGeneric<T>>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_ONE
                },
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.DOUBLE_AMOUNT
                });

            return BuildNonAllocBroadcasterGeneric(subscriptionsPool);
        }

        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            AllocationCommandDescriptor initial,
            AllocationCommandDescriptor additional)
        {
            Func<IInvokableSingleArgGeneric<T>> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableSingleArgGeneric<T>>;

            var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableSingleArgGeneric<T>>(
                valueAllocationDelegate,
                new []
                {
                    PoolsFactory.BuildIndexedMetadataDescriptor()
                },
                initial,
                additional);

            return BuildNonAllocBroadcasterGeneric(subscriptionsPool);
        }
        
        public static NonAllocBroadcasterGeneric<T> BuildNonAllocBroadcasterGeneric<T>(
            INonAllocDecoratedPool<IInvokableSingleArgGeneric<T>> subscriptionsPool)
        {
            var contents = ((IModifiable<INonAllocPool<IInvokableSingleArgGeneric<T>>>)subscriptionsPool).Contents;
			
            return new NonAllocBroadcasterGeneric<T>(
                subscriptionsPool,
                contents);
        }
        
        #endregion
    }
}