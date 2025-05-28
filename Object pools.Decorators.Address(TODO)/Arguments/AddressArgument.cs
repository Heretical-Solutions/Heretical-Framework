namespace HereticalSolutions.ObjectPools.Decorators.Address
{
    public class AddressArgument<TUUID> : IPoolPopArgument
    {
        public string FullAddress;

        public TUUID UUID;
    }
}