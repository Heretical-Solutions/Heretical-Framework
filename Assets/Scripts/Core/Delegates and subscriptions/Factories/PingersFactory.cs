using System;

using HereticalSolutions.Collections;
using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;
using HereticalSolutions.Delegates.Pinging;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

namespace HereticalSolutions.Delegates.Factories
{
	public static partial class DelegatesFactory
	{
		#region Pinger

		public static Pinger BuildPinger()
		{
			return new Pinger();
		}

		#endregion
		
		#region Non alloc pinger
		
		public static NonAllocPinger BuildNonAllocPinger()
		{
			Func<IInvokableNoArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableNoArgs>;

			var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableNoArgs>(
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

			return BuildNonAllocPinger(subscriptionsPool);
		}

		public static NonAllocPinger BuildNonAllocPinger(
			AllocationCommandDescriptor initial,
			AllocationCommandDescriptor additional)
		{
			Func<IInvokableNoArgs> valueAllocationDelegate = AllocationsFactory.NullAllocationDelegate<IInvokableNoArgs>;

			var subscriptionsPool = PoolsFactory.BuildResizableNonAllocPool<IInvokableNoArgs>(
				valueAllocationDelegate,
				new []
				{
					PoolsFactory.BuildIndexedMetadataDescriptor()
				},
				initial,
				additional);

			return BuildNonAllocPinger(subscriptionsPool);
		}

		public static NonAllocPinger BuildNonAllocPinger(INonAllocDecoratedPool<IInvokableNoArgs> subscriptionsPool)
		{
			var contents = ((IModifiable<INonAllocPool<IInvokableNoArgs>>)subscriptionsPool).Contents;
			
			return new NonAllocPinger(
				subscriptionsPool,
				contents);
		}
		
		#endregion
	}
}