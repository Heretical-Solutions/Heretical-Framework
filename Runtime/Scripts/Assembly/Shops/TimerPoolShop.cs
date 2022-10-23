using System.Collections.Generic;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Assembly.Factories;
using HereticalSolutions.Assembly.Descriptors;

namespace HereticalSolutions.Assembly
{
	public class TimerPoolShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_POOL_PROVIDABLES = "PoolProvidables";

		private const string KEY_DESCRIPTOR = "Descriptor";

		private const string KEY_PREFAB = "Prefab";

		private const string KEY_DEFAULT_DURATION = "DefaultDuration";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			var poolProvidables = (List<IPoolProvidable<GameObject>>)ticket.Arguments.Get(KEY_POOL_PROVIDABLES);

			var descriptor = ticket.Arguments.Get(KEY_DESCRIPTOR);


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];

			var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

			if (descriptor is TimerDescriptor)
			{
				var timerDescriptor = (TimerDescriptor)descriptor;

				nextShopTicket.Arguments.Add(KEY_PREFAB, (timerDescriptor).Prefab);

				nextShopTicket.Arguments.Add(KEY_DEFAULT_DURATION, (timerDescriptor).DefaultDuration);
			}

			if (descriptor is AddressTimerDescriptor)
			{
				var addressTimerDescriptor = (AddressTimerDescriptor)descriptor;

				nextShopTicket.Arguments.Add(KEY_PREFAB, (addressTimerDescriptor).Prefab);

				nextShopTicket.Arguments.Add(KEY_DEFAULT_DURATION, (addressTimerDescriptor).DefaultDuration);
			}

			if (descriptor is VariantTimerDescriptor)
			{
				var variantTimerDescriptor = (VariantTimerDescriptor)descriptor;

				nextShopTicket.Arguments.Add(KEY_PREFAB, (variantTimerDescriptor).Prefab);

				nextShopTicket.Arguments.Add(KEY_DEFAULT_DURATION, (variantTimerDescriptor).DefaultDuration);
			}

			nextShopTicket.Arguments.Remove(KEY_DESCRIPTOR);

			var nextShopResult = nextShop.Assemble(nextShopTicket, level);


			//Return
			var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

			builder
				.Add(new NonAllocPoolWithTimer(nextShopResult), out var poolWithTimer);

			poolProvidables.Add((IPoolProvidable<GameObject>)poolWithTimer);

			return builder.CurrentWrapper;
		}
	}
}