using System.Collections.Generic;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Behaviours;
using HereticalSolutions.Pools.AllocationProcessors;

using HereticalSolutions.Collections;

using HereticalSolutions.Assembly.Factories;

namespace HereticalSolutions.Assembly
{
	public class ProcessorsShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_POOL_PROVIDABLES = "PoolProvidables";

		private const string KEY_PREFAB = "Prefab";

		private const string KEY_PROCESSOR = "Processor";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			var poolProvidables = (List<IPoolProvidable<GameObject>>)ticket.Arguments.Get(KEY_POOL_PROVIDABLES);

			var prefab = (GameObject)ticket.Arguments.Get(KEY_PREFAB);


			//Create processor and add to pool providables
			NewGameObjectsPusher pusher = new NewGameObjectsPusher();

			NameByStringAndIndex namer = new NameByStringAndIndex(prefab.name);

			CompositeGameObjectAllocationProcessor processor = null;

			if (prefab.GetComponent<PoolElementBehaviour>() != null)
			{
				PoolElementBehaviourInitializer initializer = new PoolElementBehaviourInitializer();

				processor = new CompositeGameObjectAllocationProcessor(
					new Stack<IPoolElement<GameObject>>(),
					new IAllocationProcessor[]
					{
						initializer,
						pusher,
						namer
					});
			}
			else
			{
				processor = new CompositeGameObjectAllocationProcessor(
					new Stack<IPoolElement<GameObject>>(),
					new IAllocationProcessor[]
					{
						pusher,
						namer
					});
			}

			poolProvidables.Add(processor);


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];

			var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

			nextShopTicket.Arguments.Add(KEY_PROCESSOR, processor);

			return nextShop.Assemble(nextShopTicket, level);
		}
	}
}