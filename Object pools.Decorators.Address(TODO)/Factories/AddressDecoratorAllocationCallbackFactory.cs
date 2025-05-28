using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Address.Factories
{
    public class AddressDecoratorAllocationCallbackFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public AddressDecoratorAllocationCallbackFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        #region Allocation callbacks

        public SetAddressCallback<TUUIDT, T> BuildSetAddressCallback<TUUIDT, T>(
            string fullAddress)
        {
            return new SetAddressCallback<TUUIDT, T>(
                loggerResolver?.GetLogger<SetAddressCallback<TUUIDT, T>>(),
                
                fullAddress);
        }
        
        #endregion
    }
}