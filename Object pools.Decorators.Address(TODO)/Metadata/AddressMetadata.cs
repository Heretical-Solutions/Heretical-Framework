namespace HereticalSolutions.ObjectPools.Decorators.Address
{
    public class AddressMetadata<TUUID>
        : IContainsAddress<TUUID>
    {
        public string FullAddress { get; set; }

        public TUUID UUID { get; set; }

        public AddressMetadata()
        {
            FullAddress = string.Empty;
            
            UUID = default;
        }
    }
}