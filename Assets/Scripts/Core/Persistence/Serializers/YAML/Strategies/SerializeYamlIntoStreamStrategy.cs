using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeYamlIntoStreamStrategy : IYamlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string yaml)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            streamWriter.Write(yaml);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string yaml)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            yaml = string.Empty;
            
            if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
                return false;
            
            yaml = streamReader.ReadToEnd();
            
            StreamIO.CloseStream(streamReader);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(fileSystemSettings);
        }
    }
}