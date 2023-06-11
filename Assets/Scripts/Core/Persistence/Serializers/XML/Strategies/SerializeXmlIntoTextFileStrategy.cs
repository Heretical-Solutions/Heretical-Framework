using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.IO;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeXmlIntoTextFileStrategy : IXmlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

            string xml;
            
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                xml = stringWriter.ToString();
            }
            
            return TextFileIO.Write(fileSystemSettings, xml);
        }

        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;

            bool result = TextFileIO.Read(fileSystemSettings, out string xml);

            if (!result)
            {
                value = default(object);
                
                return false;
            }

            using (StringReader stringReader = new StringReader(xml))
            {
                value = serializer.Deserialize(stringReader);
            }

            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            FileSystemSettings fileSystemSettings = ((TextFileArgument)argument).Settings;
            
            TextFileIO.Erase(fileSystemSettings);
        }
    }
}