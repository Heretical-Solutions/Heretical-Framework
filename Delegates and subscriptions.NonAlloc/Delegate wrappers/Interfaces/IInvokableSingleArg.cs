using System;

namespace HereticalSolutions.Delegates.NonAlloc
{
    public interface IInvokableSingleArg
    {
        Type ValueType { get; }

        void Invoke<TArgument>(
            TArgument value);

        void Invoke(
            Type valueType,
            object value);
    }
}