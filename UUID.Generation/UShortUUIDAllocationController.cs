using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.UUID.Generation
{
	public class UShortUUIDAllocationController
		: IUUIDAllocationController<ushort>
	{
		public const ushort INVALID_VALUE = 0;

		private readonly Queue<ushort> freeIDs;

		private readonly ILogger logger;

		private ushort lastAllocatedID;

		public UShortUUIDAllocationController(
			Queue<ushort> freeIDs,
			ILogger logger)
		{
			this.freeIDs = freeIDs;

			this.logger = logger;

			lastAllocatedID = INVALID_VALUE;
		}

		#region IUUIDAllocationController

		public bool ValidateUUID(
			ushort id)
		{
			return id != INVALID_VALUE;
		}

		public bool IsAllocated(
			ushort id)
		{
			if (id == INVALID_VALUE)
				return false;

			if (id > lastAllocatedID)
			{
				return false;
			}

			if (freeIDs.Contains(
				id))
			{
				return false;
			}

			return true;
		}

		public bool AllocateUUID(
			out ushort id)
		{
			if (freeIDs.Count > 0)
			{
				id = freeIDs.Dequeue();

				return true;
			}

			if (lastAllocatedID == ushort.MaxValue)
			{
				logger?.LogError(
					GetType(),
					"NO FREE HANDLES");

				id = INVALID_VALUE;

				return false;
			}

			id = (ushort)(lastAllocatedID + 1);

			lastAllocatedID = id;

			return true;
		}

		public bool FreeUUID(
			ushort id)
		{
			if (!IsAllocated(id))
			{
				return false;
			}

			freeIDs.Enqueue(id);

			return true;
		}

		#endregion
	}
}