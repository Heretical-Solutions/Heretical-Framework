using System;

namespace HereticalSolutions.Delegates
{
    public interface ISubscribableSingleArgGeneric<TValue>
    {
        void Subscribe(Action<TValue> @delegate);
        
        void Unsubscribe(Action<TValue> @delegate);
    }
}