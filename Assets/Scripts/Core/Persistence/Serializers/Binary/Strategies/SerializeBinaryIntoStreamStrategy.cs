using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeBinaryIntoStreamStrategy : IBinarySerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, BinaryFormatter formatter, object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out FileStream fileStream))
                return false;
            
            formatter.Serialize(fileStream, value);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, BinaryFormatter formatter, out object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenReadStream(fileSystemSettings, out FileStream fileStream))
            {
                value = default(object);
                
                return false;
            }

            value = formatter.Deserialize(fileStream);
            
            StreamIO.CloseStream(fileStream);

            return true;
        }

        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            StreamIO.Erase(fileSystemSettings);
        }
    }
}