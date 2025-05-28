using System.Collections.Generic;

namespace HereticalSolutions.UUID.Mapping
{
	public interface IUUIDRegistry<TUUID>
	{
		#region Has

		bool HasUUID(
			TUUID uuid);

		bool HasPath(
			string path);

		#endregion

		#region Get

		bool TryGetUUIDByPath(
			string path,
			out TUUID uuid);

		bool TryGetPathByUUID(
			TUUID uuid,
			out string path);

		#endregion

		#region Add

		bool TryAdd(
			string path,
			TUUID uuid);

		bool TryModifyPath(
			TUUID uuid,
			string path);

		bool TryModifyUUID(
			string path,
			TUUID uuid);

		#endregion

		#region Remove

		bool TryRemove(
			TUUID uuid);

		bool TryRemove(
			string path);

		#endregion

		#region All

		//IEnumerable<AddressDescriptor> AllValues { get; }

		IEnumerable<TUUID> AllUUIDs { get; }

		IEnumerable<string> AllPaths { get; }

		#endregion
	}
}