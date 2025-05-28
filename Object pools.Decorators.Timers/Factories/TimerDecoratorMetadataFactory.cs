using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Decorators.Timers.Factories
{
    public class TimerDecoratorMetadataFactory
    {
        public AllocatedTimerMetadata BuildAllocatedTimerMetadata()
        {
            return new AllocatedTimerMetadata();
        }

        public MetadataAllocationDescriptor
            BuildAllocatedTimerMetadataMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsAllocatedTimer),
                ConcreteType = typeof(AllocatedTimerMetadata)
            };
        }
    }
}