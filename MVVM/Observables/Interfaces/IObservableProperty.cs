namespace HereticalSolutions.MVVM
{
    public interface IObservableProperty<T>
        : IPropertyChangedNotifier<T>,
          IValuePoller
    {
        T Value { get; set; }
        
        object ObjectValue { get; set; }
        
        void Clear();
    }
}