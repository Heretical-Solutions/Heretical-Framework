namespace HereticalSolutions.Persistence.Serializers
{
    public interface IYamlSerializationStrategy
    {
        bool Serialize(ISerializationArgument argument, string yaml);
        
        bool Deserialize(ISerializationArgument argument, out string yaml);

        void Erase(ISerializationArgument argument);
    }
}