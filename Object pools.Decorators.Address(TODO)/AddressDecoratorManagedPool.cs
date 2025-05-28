using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.UUID.Generation;
using HereticalSolutions.UUID.Mapping;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Address
{
    public class AddressDecoratorManagedPool<TUUID, T>
        : IManagedPool<T>,
          ICleanuppable,
          IDisposable
    {
        private readonly IUUIDAllocationController<TUUID> uuidAllocationController;

        private readonly IRepository<TUUID, IManagedPool<T>> innerPoolRepository;

        private readonly IUUIDRegistry<TUUID> uuidRegistry;

        private readonly ILogger logger;

        public AddressDecoratorManagedPool(
            IUUIDAllocationController<TUUID> uuidAllocationController,
            IRepository<TUUID, IManagedPool<T>> innerPoolRepository,
            IUUIDRegistry<TUUID> uuidRegistry,
            ILogger logger)
        {
            this.uuidAllocationController = uuidAllocationController;

            this.innerPoolRepository = innerPoolRepository;

            this.uuidRegistry = uuidRegistry;

            this.logger = logger;
        }

        #region IManagedPool

        public IPoolElementFacade<T> Pop()
        {
            throw new Exception(
                logger.TryFormatException(
                    GetType(),
                    "ADDRESS ARGUMENT ABSENT"));
        }

        public IPoolElementFacade<T> Pop(
            IPoolPopArgument[] args)
        {
            #region Validation

            if (args == null
                || !args.TryGetArgument<AddressArgument<TUUID>>(
                    out var arg))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "ADDRESS ARGUMENT ABSENT"));

            #endregion

            TUUID uuid = arg.UUID;

            if (uuid.Equals(default(TUUID))
                && !string.IsNullOrEmpty(arg.FullAddress))
            {
                if (!uuidRegistry.TryGetUUIDByPath(
                    arg.FullAddress,
                    out uuid))
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID UUID {{ {uuid} }}"));
                }
            }

            if (!innerPoolRepository.TryGet(
                    uuid,
                    out var poolByAddress))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"NO POOL BY UUID {{ {uuid} }} ADDRESS {{ {arg.FullAddress} }}"));

            var result = poolByAddress.Pop(args);

            return result;
        }

        public void Push(
            IPoolElementFacade<T> instance)
        {
            IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
                instance as IPoolElementFacadeWithMetadata<T>;

            if (instanceWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }

            if (!instanceWithMetadata.Metadata.Has<IContainsAddress<TUUID>>())
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO ADDRESS METADATA"));

            //POTENTIAL ~17B ALLOCATION HERE (Dictionary<TKey, TValue>.Get())
            var addressMetadata = instanceWithMetadata
                .Metadata
                .Get<IContainsAddress<TUUID>>();

            TUUID uuid = addressMetadata.UUID;

            if (uuid.Equals(default(TUUID))
                && !string.IsNullOrEmpty(addressMetadata.FullAddress))
            {
                if (!uuidRegistry.TryGetUUIDByPath(
                    addressMetadata.FullAddress,
                    out uuid))
                {
                    throw new Exception(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID UUID {{ {uuid} }}"));
                }

                var addressMetadataWithUUID =
                    (AddressMetadata<TUUID>)addressMetadata;

                addressMetadataWithUUID.UUID = uuid;
            }

            if (!innerPoolRepository.TryGet(
                uuid,
                out var pool))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"NO POOL BY UUID {{ {uuid} }} ADDRESS {{ {addressMetadata.FullAddress} }}"));

            pool.Push(instance);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerPoolRepository is ICleanuppable)
                (innerPoolRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerPoolRepository is IDisposable)
                (innerPoolRepository as IDisposable).Dispose();
        }

        #endregion

        public IRepository<TUUID, IManagedPool<T>> InnerPoolRepository =>
            innerPoolRepository;

        public void AddPool(
            string address,
            IManagedPool<T> pool)
        {
            if (!uuidAllocationController.AllocateUUID(
                out var uuid))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "NO FREE UUID"));
            }

            if (!uuidRegistry.TryAdd(
                address,
                uuid))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID UUID {{ {uuid} }}"));
            }

            if (!innerPoolRepository.TryAdd(
                uuid,
                pool))
            {
                uuidRegistry.TryRemove(
                    address);

                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID UUID {{ {uuid} }}"));
            }
        }

        public void RemovePool(
            string address)
        {
            if (!uuidRegistry.TryGetUUIDByPath(
                address,
                out var uuid))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID UUID {{ {uuid} }}"));
            }

            if (!uuidRegistry.TryRemove(
                address))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID UUID {{ {uuid} }}"));
            }

            if (!innerPoolRepository.TryRemove(
                uuid))
            {
                uuidRegistry.TryAdd(
                    address,
                    uuid);

                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID UUID {{ {uuid} }}"));
            }
        }
    }
}