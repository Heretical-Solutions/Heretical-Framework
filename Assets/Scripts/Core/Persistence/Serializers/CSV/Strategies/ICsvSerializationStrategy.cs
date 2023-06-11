using System;

namespace HereticalSolutions.Persistence.Serializers
{
    public interface ICsvSerializationStrategy
    {
        bool Serialize(ISerializationArgument argument, Type valueType, object value);
        
        bool Deserialize(ISerializationArgument argument, Type valueType, out object value);

        void Erase(ISerializationArgument argument);
    }
}