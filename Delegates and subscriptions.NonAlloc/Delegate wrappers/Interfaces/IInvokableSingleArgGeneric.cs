namespace HereticalSolutions.Delegates.NonAlloc
{
    public interface IInvokableSingleArgGeneric<TValue>
    {
        void Invoke(
            TValue arg);

        void Invoke(
            object arg);
    }
}