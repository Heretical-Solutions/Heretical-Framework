using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity
{
	public class SetPrefabAllocationCallback
		: IAllocationCallback<IPoolElementFacade<GameObject>>
	{
		private readonly GameObject prefab;

		private readonly ILogger logger;

		public SetPrefabAllocationCallback(
			GameObject prefab,
			ILogger logger)
		{
			this.prefab = prefab;

			this.logger = logger;
		}

		public void OnAllocated(
			IPoolElementFacade<GameObject> poolElementFacade)
		{
			IPoolElementFacadeWithMetadata<GameObject> facadeWithMetadata =
				poolElementFacade as IPoolElementFacadeWithMetadata<GameObject>;

			if (facadeWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}

			var metadata = (PrefabMetadata)
				facadeWithMetadata.Metadata.Get<IContainsPrefab>();

			if (metadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO PREFAB METADATA"));
			}

			metadata.Prefab = prefab;
		}
	}
}