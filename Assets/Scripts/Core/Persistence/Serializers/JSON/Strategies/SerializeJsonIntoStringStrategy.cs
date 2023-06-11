using HereticalSolutions.Persistence.Arguments;

namespace HereticalSolutions.Persistence.Serializers
{
    public class SerializeJsonIntoStringStrategy : IJsonSerializationStrategy
    {
        public bool Serialize(ISerializationArgument argument, string json)
        {
            ((StringArgument)argument).Value = json;

            return true;
        }

        public bool Deserialize(ISerializationArgument argument, out string json)
        {
            json = ((StringArgument)argument).Value;
            
            return true;
        }
        
        public void Erase(ISerializationArgument argument)
        {
            ((StringArgument)argument).Value = string.Empty;
        }
    }
}