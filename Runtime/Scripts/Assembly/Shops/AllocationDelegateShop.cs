using System;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;
using HereticalSolutions.Pools.AllocationProcessors;

using HereticalSolutions.Timers;

using HereticalSolutions.Collections;

using HereticalSolutions.Assembly.Factories;

using HereticalSolutions.Messaging;

namespace HereticalSolutions.Assembly
{
	public class AllocationDelegateShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_PROCESSOR = "Processor";

		private const string KEY_ADDRESS_HASHES = "AddressHashes";

		private const string KEY_VARIANT = "Variant";

		private const string KEY_DEFAULT_DURATION = "DefaultDuration";

		private const string KEY_PINGER = "Pinger";

		private const string KEY_CONTAINER_ALLOCATION_DELEGATE = "ContainerAllocationDelegate";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			CompositeGameObjectAllocationProcessor processor = (ticket.Arguments.Has(KEY_PROCESSOR))
				? (CompositeGameObjectAllocationProcessor)ticket.Arguments.Get(KEY_PROCESSOR)
				: null;

			int[] addressHashes = (ticket.Arguments.Has(KEY_ADDRESS_HASHES))
				? (int[])ticket.Arguments.Get(KEY_ADDRESS_HASHES)
				: new int[0];

			int variant = (ticket.Arguments.Has(KEY_VARIANT))
				? (int)ticket.Arguments.Get(KEY_VARIANT)
				: -1;

			float defaultDuration = (ticket.Arguments.Has(KEY_DEFAULT_DURATION))
				? (float)ticket.Arguments.Get(KEY_DEFAULT_DURATION)
				: -1f;

			Pinger pinger = (ticket.Arguments.Has(KEY_PINGER))
				? (Pinger)ticket.Arguments.Get(KEY_PINGER)
				: null;


			//Check conditions
			bool hasTimer = 
				(defaultDuration > 0f)
				&& (pinger != null);

			bool hasAddressOrDuration = 
				(addressHashes != null) 
				|| (variant != -1);

			bool hasProcessor = processor != null;


			//Create delegate
			Func<Func<GameObject>, IPoolElement<GameObject>> containerAllocationDelegate = null;

			if (hasProcessor
				&& !hasTimer
				&& !hasAddressOrDuration)
				containerAllocationDelegate = PoolFactory.BuildValueAssignedNotifyingPoolElementAllocationDelegate<GameObject>(processor);

			if (hasProcessor
				&& !hasTimer
				&& hasAddressOrDuration)
				containerAllocationDelegate = PoolFactory.BuildPoolElementWithAddressAndVariantAllocationDelegate<GameObject>(
					processor,
					addressHashes,
					variant);

			if (hasProcessor
				&& hasTimer)
			containerAllocationDelegate = PoolFactory.BuildPoolElementWithTimerAllocationDelegate<GameObject>(
					processor,
					() =>
					{
						return new Timer(
							defaultDuration,
							pinger);
					},
					addressHashes,
					variant);


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];

			var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

			nextShopTicket.Arguments.Add(KEY_CONTAINER_ALLOCATION_DELEGATE, containerAllocationDelegate);

			return nextShop.Assemble(nextShopTicket, level);
		}
	}
}