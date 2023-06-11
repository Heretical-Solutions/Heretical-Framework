using System.Runtime.Serialization.Formatters.Binary;

namespace HereticalSolutions.Persistence.Serializers
{
    public interface IBinarySerializationStrategy
    {
        bool Serialize(ISerializationArgument argument, BinaryFormatter formatter, object value);

        bool Deserialize(ISerializationArgument argument, BinaryFormatter formatter, out object value);

        void Erase(ISerializationArgument argument);
    }
}