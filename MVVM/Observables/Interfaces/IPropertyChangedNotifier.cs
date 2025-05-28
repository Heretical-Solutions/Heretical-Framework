using System;

namespace HereticalSolutions.MVVM
{
    public interface IPropertyChangedNotifier<T>
    {
        Action<T> OnValueChanged { get; set; }
        
        void RemoveAllListeners();
    }
}