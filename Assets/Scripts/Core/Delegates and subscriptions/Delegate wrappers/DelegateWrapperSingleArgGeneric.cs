using System;

namespace HereticalSolutions.Delegates.Wrappers
{
    public class DelegateWrapperSingleArgGeneric<TValue>
        : IInvokableSingleArgGeneric<TValue>,
          IInvokableSingleArg
    {
        private readonly Action<TValue> @delegate;

        public DelegateWrapperSingleArgGeneric(Action<TValue> @delegate)
        {
            this.@delegate = @delegate;
        }

        #region IInvokableSingleArgGeneric
        
        public void Invoke(TValue argument)
        {
            @delegate?.Invoke(argument);
        }

        public void Invoke(object argument)
        {
            @delegate?.Invoke((TValue)argument);
        }
        
        #endregion

        #region IInvokableSingleArg
        
        public void Invoke<TArgument>(TArgument value)
        {
            if (!(typeof(TArgument).Equals(typeof(TValue))))
                throw new Exception($"[DelegateWrapperSingleArgGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
			
            Invoke((object)value); //It doesn't want to convert TArgument into TValue. Bastard
        }

        public void Invoke(Type valueType, object value)
        {
            if (!(valueType.Equals(typeof(TValue))))
                throw new Exception($"[DelegateWrapperSingleArgGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");
			
            Invoke(value); //It doesn't want to convert TArgument into TValue. Bastard
        }
        
        #endregion
    }
}