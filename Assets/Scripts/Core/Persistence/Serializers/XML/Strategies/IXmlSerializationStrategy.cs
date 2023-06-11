using System.Xml.Serialization;

namespace HereticalSolutions.Persistence.Serializers
{
    public interface IXmlSerializationStrategy
    {
        bool Serialize(ISerializationArgument argument, XmlSerializer serializer, object value);
        
        bool Deserialize(ISerializationArgument argument, XmlSerializer serializer, out object value);

        void Erase(ISerializationArgument argument);
    }
}