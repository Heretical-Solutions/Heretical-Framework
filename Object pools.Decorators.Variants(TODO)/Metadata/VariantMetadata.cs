namespace HereticalSolutions.ObjectPools.Decorators.Variants
{
    public class VariantMetadata : IContainsVariant
    {
        public int Variant { get; set; }

        public VariantMetadata()
        {
            Variant = -1;
        }
    }
}