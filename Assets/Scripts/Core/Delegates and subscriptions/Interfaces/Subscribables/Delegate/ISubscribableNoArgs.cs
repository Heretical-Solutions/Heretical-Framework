using System;

namespace HereticalSolutions.Delegates
{
    public interface ISubscribableNoArgs
    {
        void Subscribe(Action @delegate);
        
        void Unsubscribe(Action @delegate);
    }
}