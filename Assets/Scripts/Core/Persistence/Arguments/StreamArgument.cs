using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Arguments
{
    public class StreamArgument : ISerializationArgument
    {
        public FileSystemSettings Settings { get; set; }
    }
}