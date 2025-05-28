using System;
using System.Collections.Generic;

namespace HereticalSolutions.MVVM
{
    public delegate bool PollValueDelegate<T>(
        T previousValue,
        out T newValue);
    
    public delegate void ProvideValueDelegate<T>(
        T newValue);

    public class ObservableProperty<T>
        : IObservableProperty<T>
    {
        private T value;

        private Action<T> onValueChanged;
        
        private PollValueDelegate<T> pollValueDelegate;
        
        private ProvideValueDelegate<T> provideValueDelegate; 
        
        public ObservableProperty(
            T defaultValue = default(T))
        {
            value = defaultValue;
            
            pollValueDelegate = null;
            provideValueDelegate = null;

            RemoveAllListeners();
        }
        
        public ObservableProperty(
            T defaultValue = default(T),
            PollValueDelegate<T> pollValueDelegate = null,
            ProvideValueDelegate<T> provideValueDelegate = null)
        {
            value = defaultValue;
            this.pollValueDelegate = pollValueDelegate;
            this.provideValueDelegate = provideValueDelegate;

            RemoveAllListeners();
            
            if (pollValueDelegate != null)
                PollValue();
        }

        public ObservableProperty(
            T defaultValue = default(T),
            Func<T> pollValueFuncDelegate = null,
            Action<T> provideValueActionDelegate = null)
        {
            value = defaultValue;
            
            if (pollValueFuncDelegate == null)
                pollValueDelegate = null;
            else
                pollValueDelegate =
                    (T previousValue, out T result) =>
                    {
                        result = pollValueFuncDelegate();

                        //Courtesy of https://stackoverflow.com/questions/488250/c-sharp-compare-two-generic-values
                        return !EqualityComparer<T>.Default.Equals(previousValue, result);
                    };
            
            if (provideValueActionDelegate == null)
                provideValueDelegate = null;
            else
                provideValueDelegate = (result) =>
                {
                    provideValueActionDelegate(result);
                };

            RemoveAllListeners();

            
            if (pollValueDelegate != null)
                PollValue();
        }
        
        #region IObservableProperty

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                
                provideValueDelegate?.Invoke(value);
                
                onValueChanged?.Invoke(value);
            }
        }
        
        public object ObjectValue
        {
            get => value;
            set
            {
                this.value = (T)value;
                
                provideValueDelegate?.Invoke((T)value);
                
                onValueChanged?.Invoke((T)value);
            }
        }

        public void Clear()
        {
            value = default(T);

            pollValueDelegate = null;

            provideValueDelegate = null;

            RemoveAllListeners();
        }

        #endregion

        #region IPropertyModifiedNotifiable

        //Getting or modifying the delegate comes with allocation costs
        //While invocation does not
        //So for the delegates I'll leave it as is so that we can invoke them without allocations
        public Action<T> OnValueChanged
        {
            get => onValueChanged;
            set => onValueChanged = value;
        }

        public void RemoveAllListeners()
        {
            onValueChanged = null;
        }

        #endregion

        #region IValuePollable

        public void PollValue()
        {
            if (pollValueDelegate != null
                && pollValueDelegate(
                    value,
                    out value))
            {
                provideValueDelegate?.Invoke(value);
                
                onValueChanged?.Invoke(value);
            }
        }

        #endregion
    }
}