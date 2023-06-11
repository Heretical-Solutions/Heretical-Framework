using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeJsonIntoStreamStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            streamWriter.Write(json);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            json = string.Empty;
            
            if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
                return false;

            json = streamReader.ReadToEnd();
            
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