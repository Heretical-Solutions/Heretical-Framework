using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public class SerializerContext
		: ISerializerContext
	{
		public IVisitor Visitor { get; set; }

		public IFormatSerializer FormatSerializer { get; set; }

		public  IDataConverter DataConverter { get; set; }

		public ISerializationMedium SerializationMedium { get; set; }

		public IStronglyTypedMetadata Arguments { get; set; }
	}
}