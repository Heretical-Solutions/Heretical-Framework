namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitorGeneric<TValue, TDTO>
    {
        bool Save(TValue value, out TDTO DTO);
    }
}