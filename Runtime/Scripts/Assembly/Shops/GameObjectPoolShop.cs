using System;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;

using Zenject;

namespace HereticalSolutions.Assembly
{
	public class GameObjectPoolShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_COLLECTION_TYPE = "CollectionType";

		private const string KEY_PREFAB = "Prefab";

		private const string KEY_CONTAINER = "Container";

		private const string KEY_POOL_PARENT = "PoolParent";

		private const string KEY_INIITIAL_ALLOCATION = "InitialAllocation";

		private const string KEY_ADDITIONAL_ALLOCATION = "AdditionalAllocation";

		private const string KEY_CONTAINER_ALLOCATION_DELEGATE = "ContainerAllocationDelegate";

		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			var collectionType = (Type)ticket.Arguments.Get(KEY_COLLECTION_TYPE);

			var prefab = (GameObject)ticket.Arguments.Get(KEY_PREFAB);

			var container = (DiContainer)ticket.Arguments.Get(KEY_CONTAINER);

			var poolParent = (Transform)ticket.Arguments.Get(KEY_POOL_PARENT);
			
			var initialAllocation = (AllocationCommandDescriptor)ticket.Arguments.Get(KEY_INIITIAL_ALLOCATION);

			var additionalAllocation = (AllocationCommandDescriptor)ticket.Arguments.Get(KEY_ADDITIONAL_ALLOCATION);

			var containerAllocationDelegate = (Func<Func<GameObject>, IPoolElement<GameObject>>)ticket.Arguments.Get(KEY_CONTAINER_ALLOCATION_DELEGATE);


			//Build and return game object pool
			INonAllocPool<GameObject> packedArrayPool = PoolFactory.BuildGameObjectPool(
				new BuildNonAllocGameObjectPoolCommand
				{
					CollectionType = collectionType,
					Prefab = prefab,
					Container = container,
					InitialAllocation = initialAllocation,
					AdditionalAllocation = additionalAllocation,
					ContainerAllocationDelegate = containerAllocationDelegate
				});

			var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

			builder
				.Add(new NonAllocWrapperPool<GameObject>(packedArrayPool))
				.Add(new NonAllocGameObjectPool(builder.CurrentWrapper, poolParent));

			return builder.CurrentWrapper;
		}
	}
}