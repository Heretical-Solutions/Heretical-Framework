using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Variants
{
    public class SetVariantCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        public int Variant { get; set; }

        private ILogger logger;
        
        public SetVariantCallback(
            ILogger logger,
            
            int variant = -1)
        {
            Variant = variant;

            this.logger = logger;
        }
        
        public void OnAllocated(IPoolElementFacade<T> poolElementFacade)
        {
            IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
                poolElementFacade as IPoolElementFacadeWithMetadata<T>;

            if (facadeWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
            
            var metadata = (VariantMetadata)
                facadeWithMetadata.Metadata.Get<IContainsVariant>();
            
            if (metadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO VARIANT METADATA"));
            }
            
            metadata.Variant = Variant;
        }
    }
}