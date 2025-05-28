using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;

namespace HereticalSolutions.UUID.Generation.Factories
{
    public class UUIDAllocationFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public UUIDAllocationFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        public ByteUUIDAllocationController BuildByteUUIDAllocationController()
        {
            ILogger logger = loggerResolver?.GetLogger<ByteUUIDAllocationController>();

            return new ByteUUIDAllocationController(
                new Queue<byte>(),
                logger);
        }

        public UShortUUIDAllocationController BuildUShortUUIDAllocationController()
        {
            ILogger logger = loggerResolver?.GetLogger<ByteUUIDAllocationController>();

            return new UShortUUIDAllocationController(
                new Queue<ushort>(),
                logger);
        }

        public GUIDAllocationController BuildGUIDAllocationController()
        {
            return new GUIDAllocationController(
                new HashSet<Guid>());
        }
    }
}