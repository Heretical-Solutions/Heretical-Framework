using System;

namespace HereticalSolutions.Delegates
{
    public interface ISubscribableMultipleArgs
    {
        void Subscribe(Action<object[]> @delegate);

        void Unsubscribe(Action<object[]> @delegate);
    }
}