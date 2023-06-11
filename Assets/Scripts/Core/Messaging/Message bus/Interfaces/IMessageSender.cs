using System;

namespace HereticalSolutions.Messaging
{
    public interface IMessageSender
    {
        #region Pop
        
        IMessageSender PopMessage(Type messageType, out IMessage message);

        IMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : IMessage;
        
        #endregion

        #region Write
        
        IMessageSender Write(IMessage message, object[] args);
        
        IMessageSender Write<TMessage>(TMessage message, object[] args) where TMessage : IMessage;
        
        #endregion

        #region Send
        
        void Send(IMessage message);

        void Send<TMessage>(TMessage message) where TMessage : IMessage;

        void SendImmediately(IMessage message);

        void SendImmediately<TMessage>(TMessage message) where TMessage : IMessage;
        
        #endregion

        #region Deliver
        
        void DeliverMessagesInMailbox();
        
        #endregion
    }
}