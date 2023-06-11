using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Messaging
{
    public class MessageBus
        : IMessageSender, 
	      IMessageReceiver
    {
	    private readonly BroadcasterWithRepository broadcaster;

        private readonly IReadOnlyObjectRepository messageRepository;

        private readonly Queue<IMessage> mailbox;

        public MessageBus(
	        BroadcasterWithRepository broadcaster,
	        IReadOnlyObjectRepository messageRepository,
			Queue<IMessage> mailbox)
        {
            this.broadcaster = broadcaster;

            this.messageRepository = messageRepository;

            this.mailbox = mailbox;
        }

        #region IMessageSender

        #region Pop
        
        public IMessageSender PopMessage(Type messageType, out IMessage message)
        {
	        if (!messageRepository.TryGet(
		            messageType,
		            out object messagePoolObject))
		        throw new Exception($"[MessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

	        IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;
	        
	        message = messagePool.Pop();

	        return this;
        }

        public IMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : IMessage
        {
	        if (!messageRepository.TryGet(
		            typeof(TMessage),
		            out object messagePoolObject))
		        throw new Exception($"[MessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

	        IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;
	        
	        message = (TMessage)messagePool.Pop();

	        return this;
        }

        #endregion
        
        #region Write
        
        public IMessageSender Write(IMessage message, object[] args)
        {
	        if (message == null)
		        throw new Exception($"[MessageBus] INVALID MESSAGE");

	        message.Write(args);

	        return this;
        }

        public IMessageSender Write<TMessage>(TMessage message, object[] args) where TMessage : IMessage
        {
	        if (message == null)
		        throw new Exception($"[MessageBus] INVALID MESSAGE");

	        message.Write(args);

	        return this;
        }

        #endregion
        
        #region Send
        
        public void Send(IMessage message)
        {
	        mailbox.Enqueue(message);
        }

        public void Send<TMessage>(TMessage message) where TMessage : IMessage
        {
	        mailbox.Enqueue(message);
        }

        public void SendImmediately(IMessage message)
        {
	        broadcaster.Publish(message.GetType(), message);

	        PushMessageToPool(message);
        }
        
        public void SendImmediately<TMessage>(TMessage message) where TMessage : IMessage
        {
	        broadcaster.Publish<TMessage>(message);

	        PushMessageToPool<TMessage>(message);
        }
        
        #endregion
        
        #region Deliver
        
        public void DeliverMessagesInMailbox()
        {
	        int messagesToReceive = mailbox.Count;

	        for (int i = 0; i < messagesToReceive; i++)
	        {
		        var message = mailbox.Dequeue();

		        SendImmediately(message);
	        }
        }
        
        #endregion
        
        private void PushMessageToPool(IMessage message)
        {
	        var messageType = message.GetType();

	        if (!messageRepository.TryGet(
		            messageType,
		            out object messagePoolObject))
		        throw new Exception($"[MessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

	        IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;
	        
	        messagePool.Push(message);
        }

        private void PushMessageToPool<TMessage>(TMessage message) where TMessage : IMessage
        {
	        var messageType = typeof(TMessage);
	        
	        if (!messageRepository.TryGet(
		            messageType,
		            out object messagePoolObject))
		        throw new Exception($"[MessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

	        IPool<IMessage> messagePool = (IPool<IMessage>)messagePoolObject;
	        
	        messagePool.Push(message);
        }

        #endregion

        #region IMessageReceiver
        
        public void SubscribeTo<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage
        {
	        broadcaster.Subscribe<TMessage>(receiverDelegate);
        }
        
        public void SubscribeTo(Type messageType, object receiverDelegate)
        {
	        broadcaster.Subscribe(messageType, receiverDelegate);
        }

        public void UnsubscribeFrom<TMessage>(Action<TMessage> receiverDelegate) where TMessage : IMessage
        {
	        broadcaster.Unsubscribe<TMessage>(receiverDelegate);
        }

        public void UnsubscribeFrom(Type messageType, object receiverDelegate)
        {
	        broadcaster.Unsubscribe(messageType, receiverDelegate);
        }

        #endregion
    }
}