using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeYamlIntoTextFileStrategy : IYamlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(fileSystemSettings, yaml);
        }

        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(fileSystemSettings, out yaml);
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(fileSystemSettings);
        }
    }
}