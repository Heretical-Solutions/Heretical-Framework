namespace HereticalSolutions.MVVM
{
    public interface IViewModel
    {
        #region Get observable

        bool TryGetObservable<T>(
            string key,
            out IObservableProperty<T> observable);

        #endregion

        #region Get command

        CommandDelegate GetCommand(
            string key);
        
        CommandWithArgsDelegate GetCommandWithArguments(
            string key);
        
        #endregion
    }
}