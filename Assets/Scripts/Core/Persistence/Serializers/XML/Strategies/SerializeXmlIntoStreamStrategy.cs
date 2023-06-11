using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeXmlIntoStreamStrategy : IXmlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;
            
            if (!StreamIO.OpenWriteStream(fileSystemSettings, out StreamWriter streamWriter))
                return false;
            
            serializer.Serialize(streamWriter, value);
            
            StreamIO.CloseStream(streamWriter);

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            FileSystemSettings fileSystemSettings = ((StreamArgument)argument).Settings;

            if (!StreamIO.OpenReadStream(fileSystemSettings, out StreamReader streamReader))
            {
                value = default(object);
                
                return false;
            }

            value = serializer.Deserialize(streamReader);
            
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