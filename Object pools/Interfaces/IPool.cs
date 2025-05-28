namespace HereticalSolutions.ObjectPools
{
    public interface IPool<T>
    {
        T Pop();

        T Pop(
            IPoolPopArgument[] args);
        
        void Push(
            T instance);
    }
}