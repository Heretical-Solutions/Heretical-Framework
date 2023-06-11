using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeJsonIntoTextFileStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Write(fileSystemSettings, json);
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            return TextFileIO.Read(fileSystemSettings, out json);
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(fileSystemSettings);
        }
    }
}