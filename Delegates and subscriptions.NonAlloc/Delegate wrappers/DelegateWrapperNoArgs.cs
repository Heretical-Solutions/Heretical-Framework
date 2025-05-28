using System;

namespace HereticalSolutions.Delegates.NonAlloc
{
    public class DelegateWrapperNoArgs
        : IInvokableNoArgs
    {
        private readonly Action @delegate;

        public DelegateWrapperNoArgs(
            Action @delegate)
        {
            this.@delegate = @delegate;
        }

        public void Invoke()
        {
            @delegate?.Invoke();
        }
    }
}