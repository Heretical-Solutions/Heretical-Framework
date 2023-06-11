using System;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Messaging
{
    public interface INonAllocMessageSender
    {
        #region Pop
        
        INonAllocMessageSender PopMessage(Type messageType, out IPoolElement<IMessage> message);

        INonAllocMessageSender PopMessage<TMessage>(out IPoolElement<IMessage> message) where TMessage : IMessage;
        
        #endregion

        #region Write
        
        INonAllocMessageSender Write(IPoolElement<IMessage> messageElement, object[] args);
        
        INonAllocMessageSender Write<TMessage>(IPoolElement<IMessage> messageElement, object[] args) where TMessage : IMessage;
        
        #endregion

        #region Send
        
        void Send(IPoolElement<IMessage> message);

        void Send<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage;

        void SendImmediately(IPoolElement<IMessage> message);

        void SendImmediately<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage;
        
        #endregion

        #region Deliver
        
        void DeliverMessagesInMailbox();
        
        #endregion
    }
}