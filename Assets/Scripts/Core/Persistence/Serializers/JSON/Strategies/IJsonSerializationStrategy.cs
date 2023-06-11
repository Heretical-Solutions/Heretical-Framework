namespace HereticalSolutions.Persistence.Serializers
{
    public interface IJsonSerializationStrategy
    {
        bool Serialize(ISerializationArgument argument, string json);
        
        bool Deserialize(ISerializationArgument argument, out string json);

        void Erase(ISerializationArgument argument);
    }
}