using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.UUID.Generation
{
	public class ByteUUIDAllocationController
		: IUUIDAllocationController<byte>
	{
		public const byte INVALID_VALUE = 0;

		private readonly Queue<byte> freeIDs;

		private readonly ILogger logger;

		private byte lastAllocatedID;

		public ByteUUIDAllocationController(
			Queue<byte> freeIDs,
			ILogger logger)
		{
			this.freeIDs = freeIDs;

			this.logger = logger;

			lastAllocatedID = INVALID_VALUE;
		}

		#region IUUIDAllocationController

		public bool ValidateUUID(
			byte id)
		{
			return id != INVALID_VALUE;
		}

		public bool IsAllocated(
			byte id)
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
			out byte id)
		{
			if (freeIDs.Count > 0)
			{
				id = freeIDs.Dequeue();

				return true;
			}

			if (lastAllocatedID == byte.MaxValue)
			{
				logger?.LogError(
					GetType(),
					"NO FREE HANDLES");

				id = INVALID_VALUE;

				return false;
			}
			
			id = (byte)(lastAllocatedID + 1);

			lastAllocatedID = id;

			return true;
		}

		public bool FreeUUID(
			byte id)
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