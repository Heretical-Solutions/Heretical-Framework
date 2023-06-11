using System;

using HereticalSolutions.Delegates.Wrappers;

namespace HereticalSolutions.Delegates.Factories
{
    public static partial class DelegatesFactory
    {
        #region Delegate wrappers

        public static DelegateWrapperNoArgs BuildDelegateWrapperNoArgs(Action @delegate)
        {
            return new DelegateWrapperNoArgs(@delegate);
        }
        
        public static IInvokableSingleArg BuildDelegateWrapperSingleArg<TValue>(Action<TValue> @delegate)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(@delegate);
        }
        
        public static DelegateWrapperSingleArgGeneric<TValue> BuildDelegateWrapperSingleArgGeneric<TValue>(Action<TValue> @delegate)
        {
            return new DelegateWrapperSingleArgGeneric<TValue>(@delegate);
        }
        
        public static DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(@delegate);
        }

        #endregion
    }
}