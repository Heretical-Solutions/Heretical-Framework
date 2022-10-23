using System.Collections.Generic;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Assembly.Descriptors;
using HereticalSolutions.Assembly.Factories;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Assembly
{
	public class VariantPoolShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_DESCRIPTOR = "Descriptor";

		private const string KEY_DESCRIPTORS = "Descriptors";

		private const string KEY_PREFAB = "Prefab";

		private const string KEY_VARIANT = "Variant";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			var descriptors = ticket.Arguments.Get(KEY_DESCRIPTORS);


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];


			//Create database
			Dictionary<int, VariantContainer<GameObject>> database = new Dictionary<int, VariantContainer<GameObject>>();


			//Fill database
			if (descriptors is VariantDescriptor[])
			{
				var variantDescriptors = (VariantDescriptor[])descriptors;

				for (int i = 0; i < variantDescriptors.Length; i++)
				{
					var currentVariant = variantDescriptors[i];

					var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

					nextShopTicket.Arguments.Remove(KEY_DESCRIPTORS);

					nextShopTicket.Arguments.Add(KEY_PREFAB, currentVariant.Prefab);

					nextShopTicket.Arguments.Add(KEY_VARIANT, i);

					database.Add(
						i,
						new VariantContainer<GameObject>
						{
							Chance = currentVariant.Chance,

							Pool = nextShop.Assemble(nextShopTicket, level)
						});
				}
			}

			if (descriptors is VariantTimerDescriptor[])
			{
				var variantTimerDescriptors = (VariantTimerDescriptor[])descriptors;

				for (int i = 0; i < variantTimerDescriptors.Length; i++)
				{
					var currentVariant = variantTimerDescriptors[i];

					var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

					nextShopTicket.Arguments.Remove(KEY_DESCRIPTORS);

					nextShopTicket.Arguments.Add(KEY_DESCRIPTOR, currentVariant);

					nextShopTicket.Arguments.Add(KEY_VARIANT, i);

					database.Add(
						i,
						new VariantContainer<GameObject>
						{
							Chance = currentVariant.Chance,

							Pool = nextShop.Assemble(nextShopTicket, level)
						});
				}
			}


			//Return
			var repository = new DictionaryRepository<int, VariantContainer<GameObject>>(database);

			var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

			builder
				.Add(new PoolWithVariants<GameObject>(repository));
			
			return builder.CurrentWrapper;
		}
	}
}