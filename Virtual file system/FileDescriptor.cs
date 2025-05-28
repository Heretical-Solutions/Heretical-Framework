using System;

namespace HereticalSolutions.UUID.Mapping
{
	[Serializable]
	public struct FileDescriptor
	{
		public uint Offset;

		public uint Size;

		public string DataType;
	}
}