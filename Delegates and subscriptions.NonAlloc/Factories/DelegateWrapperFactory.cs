using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.NonAlloc.Factories
{
    public class DelegateWrapperFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public DelegateWrapperFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        #region Delegate wrappers

        public DelegateWrapperNoArgs BuildDelegateWrapperNoArgs(
            Action @delegate)
        {
            return new DelegateWrapperNoArgs(
                @delegate);
        }
        
        public DelegateWrapperSingleArgGeneric<TValue> 
            BuildDelegateWrapperSingleArgGeneric<TValue>(
                Action<TValue> @delegate)
        {
            ILogger logger =
                loggerResolver?.GetLogger<DelegateWrapperSingleArgGeneric<TValue>>();

            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(
            Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(
                @delegate);
        }

        #endregion
    }
}