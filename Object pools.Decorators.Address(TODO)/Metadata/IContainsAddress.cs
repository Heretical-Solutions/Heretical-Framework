namespace HereticalSolutions.ObjectPools.Decorators.Address
{
    public interface IContainsAddress<TUUID>
    {
        string FullAddress { get; }

        TUUID UUID { get; }
    }
}