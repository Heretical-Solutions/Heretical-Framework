using System.IO;
using System.Xml.Serialization;

using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeXmlIntoStringStrategy : IXmlSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, value);
                
                ((StringArgument)argument).Value = stringWriter.ToString();
            }
            
            return true;
        }

        public bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value)
        {
            using (StringReader stringReader = new StringReader(((StringArgument)argument).Value))
            {
                value = serializer.Deserialize(stringReader);
            }
            
            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}