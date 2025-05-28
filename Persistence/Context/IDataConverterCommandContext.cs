using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public interface IDataConverterCommandContext
	{
		ISerializationMedium SerializationMedium { get; }

		IStronglyTypedMetadata Arguments { get; }
	}
}