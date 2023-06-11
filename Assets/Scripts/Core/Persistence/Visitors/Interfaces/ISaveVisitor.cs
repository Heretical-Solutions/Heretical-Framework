namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitor
    {
        bool Save<TValue>(TValue value, out object DTO);
        
        bool Save<TValue, TDTO>(TValue value, out TDTO DTO);
    }
}