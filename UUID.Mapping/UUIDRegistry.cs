using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.UUID.Mapping
{
	//TODO: symlinks, warlinks and wildcards should NOT be t he part of UUID mapping. They don't even deal with UUIDs, only paths
	//TODO: actually, the PATHING should be extended and UUID mapping should be the part of it and its own namespace
	public class UUIDRegistry<TUUID>
		: IUUIDRegistry<TUUID>
	{
		private readonly IOneToOneMap<string, TUUID> pathToUUIDMap;

		public UUIDRegistry(
			IOneToOneMap<string, TUUID> pathToUUIDMap)
		{
			this.pathToUUIDMap = pathToUUIDMap;
		}

		#region IUUIDRegistry

		#region Has

		public bool HasUUID(
			TUUID uuid)
		{
			return pathToUUIDMap.HasRight(
				uuid);
		}

		public bool HasPath(
			string path)
		{
			return pathToUUIDMap.HasLeft(
				path);
		}

		#endregion

		#region Get

		public bool TryGetUUIDByPath(
			string path,
			out TUUID uuid)
		{
			return pathToUUIDMap.TryGetRight(
				path,
				out uuid);
		}

		public bool TryGetPathByUUID(
			TUUID uuid,
			out string path)
		{
			return pathToUUIDMap.TryGetLeft(
				uuid,
				out path);
		}

		#endregion

		#region Add

		public bool TryAdd(
			string path,
			TUUID uuid)
		{
			return pathToUUIDMap.TryAdd(
				path,
				uuid);
		}

		public bool TryModifyPath(
			TUUID uuid,
			string path)
		{
			return pathToUUIDMap.TryUpdateByRight(
				path,
				uuid);
		}

		public bool TryModifyUUID(
			string path,
			TUUID uuid)
		{
			return pathToUUIDMap.TryUpdateByLeft(
				path,
				uuid);
		}

		#endregion

		#region Remove

		public bool TryRemove(
			TUUID uuid)
		{
			return pathToUUIDMap.TryRemoveByRight(
				uuid);
		}

		public bool TryRemove(
			string path)
		{
			return pathToUUIDMap.TryRemoveByLeft(
				path);
		}

		#endregion

		#region All

		/*
		public IEnumerable<AddressDescriptor> AllValues
		{
			get
			{
				foreach (var path in pathToUUIDMap.LeftValues)
				{
					if (TryGetUUIDByPath(
						path,
						out var uuid))
					{
						yield return new AddressDescriptor
						{
							UUID = uuid,

							Path = path
						};
					}
				}
			}
		}
		*/

		public IEnumerable<TUUID> AllUUIDs
		{
			get
			{
				return pathToUUIDMap.RightValues;
			}
		}

		public IEnumerable<string> AllPaths
		{
			get
			{
				return pathToUUIDMap.LeftValues;
			}
		}

		#endregion

		#endregion
	}
}