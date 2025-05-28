using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.ObjectPools.Decorators.Address
{
    public class SetAddressCallback<TUUID, T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        public string FullAddress { get; set; }

        private ILogger logger;
        
        public SetAddressCallback(
            ILogger logger,
            string fullAddress = null)
        {
            FullAddress = fullAddress;
            
            this.logger = logger;
        }

        public void OnAllocated(
            IPoolElementFacade<T> poolElementFacade)
        {
            if (string.IsNullOrEmpty(FullAddress))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "ADDRESS ARGUMENT ABSENT"));
            }

            IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
                poolElementFacade as IPoolElementFacadeWithMetadata<T>;

            if (facadeWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
			
            var metadata = (AddressMetadata<TUUID>)
                facadeWithMetadata.Metadata.Get<IContainsAddress<TUUID>>();

            if (metadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO ADDRESS METADATA"));
            }
            
            metadata.FullAddress = FullAddress;
            
            metadata.UUID = default;
        }
    }
}