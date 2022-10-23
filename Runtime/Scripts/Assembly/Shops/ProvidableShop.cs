using System.Collections.Generic;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Behaviours;
using HereticalSolutions.Pools.AllocationProcessors;

using HereticalSolutions.Collections;

using HereticalSolutions.Assembly.Factories;

namespace HereticalSolutions.Assembly
{
	public class ProvidableShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_POOL_PROVIDABLES = "PoolProvidables";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			List<IPoolProvidable<GameObject>> poolProvidables = new List<IPoolProvidable<GameObject>>();


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];

			var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

			nextShopTicket.Arguments.Add(KEY_POOL_PROVIDABLES, poolProvidables);

			var result = nextShop.Assemble(nextShopTicket, level);


			//Provide
			foreach (var poolProvidable in poolProvidables)
				poolProvidable.SetPool(result);

			
			//Return
			return result;
		}
	}
}