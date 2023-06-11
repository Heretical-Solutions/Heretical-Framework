using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Arguments
{
    public class TextFileArgument : ISerializationArgument
    {
        public FileSystemSettings Settings { get; set; }
    }
}