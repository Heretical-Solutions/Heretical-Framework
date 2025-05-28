using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Variants.Factories
{
    public class VariantDecoratorAllocationCallbackFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public VariantDecoratorAllocationCallbackFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        public SetVariantCallback<T> BuildSetVariantCallback<T>(
            int variant = -1)
        {
            ILogger logger = loggerResolver?.GetLogger<SetVariantCallback<T>>();

            return new SetVariantCallback<T>(
                logger,
                variant);
        }
    }
}