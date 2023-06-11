using System.IO;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class UnitySerializeJsonIntoStreamStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            UnityFileSystemSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            if (!UnityStreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            streamWriter.Write(json);
            
            UnityStreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            UnityFileSystemSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            json = string.Empty;
            
            if (!UnityStreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
                return false;

            json = streamReader.ReadToEnd();
            
            UnityStreamIO.CloseStream(streamReader);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            UnityFileSystemSettings fileSystemSettings = ((UnityStreamArgument)argument).Settings;
            
            UnityStreamIO.Erase(fileSystemSettings);
        }
    }
}