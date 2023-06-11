using System;

namespace HereticalSolutions.Persistence
{
    public interface ISerializer
    {
        bool Serialize<TValue>(ISerializationArgument argument, TValue DTO);
        
        bool Serialize(ISerializationArgument argument, Type DTOType, object DTO);
        
        bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO);

        bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO);
        
        void Erase(ISerializationArgument argument);
    }
}