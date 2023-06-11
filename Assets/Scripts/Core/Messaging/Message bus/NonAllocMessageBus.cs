using System;

using HereticalSolutions.Collections;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Broadcasting;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Messaging
{
    public class NonAllocMessageBus
        : INonAllocMessageSender, 
	      INonAllocMessageReceiver
    {
        private readonly NonAllocBroadcasterWithRepository broadcaster;

        private readonly IReadOnlyObjectRepository messageRepository;

        private readonly INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox;

        private readonly IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable;

        public NonAllocMessageBus(
            NonAllocBroadcasterWithRepository broadcaster,
            IReadOnlyObjectRepository messageRepository,
            INonAllocDecoratedPool<IPoolElement<IMessage>> mailbox,
            IIndexable<IPoolElement<IPoolElement<IMessage>>> mailboxContentsAsIndexable)
        {
            this.broadcaster = broadcaster;

            this.messageRepository = messageRepository;

            this.mailbox = mailbox;

            this.mailboxContentsAsIndexable = mailboxContentsAsIndexable;
        }

        #region IMessageSenderNonAlloc

        #region Pop
        
        public INonAllocMessageSender PopMessage(Type messageType, out IPoolElement<IMessage> message)
        {
            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;
	        
            message = messagePool.Pop(null);

            return this;
        }

        public INonAllocMessageSender PopMessage<TMessage>(out IPoolElement<IMessage> message) where TMessage : IMessage
        {
            if (!messageRepository.TryGet(
                    typeof(TMessage),
                    out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;
	        
            message = messagePool.Pop(null);

            return this;
        }
        
        #endregion

        #region Write

        public INonAllocMessageSender Write(IPoolElement<IMessage> messageElement, object[] args)
        {
            if (messageElement == null)
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE");

            messageElement.Value.Write(args);

            return this;
        }
        
        public INonAllocMessageSender Write<TMessage>(IPoolElement<IMessage> messageElement, object[] args) where TMessage : IMessage
        {
            if (messageElement == null)
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE");

            messageElement.Value.Write(args);

            return this;
        }
        
        #endregion

        #region Send

        public void Send(IPoolElement<IMessage> message)
        {
            var messageElement = mailbox.Pop(null);

            messageElement.Value = message;
        }

        public void Send<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            var messageElement = mailbox.Pop(null);

            messageElement.Value = message;
        }

        public void SendImmediately(IPoolElement<IMessage> message)
        {
            broadcaster.Publish(message.Value.GetType(), message.Value);

            PushMessageToPool(message);
        }

        public void SendImmediately<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            broadcaster.Publish<TMessage>((TMessage)message.Value);

            PushMessageToPool<TMessage>(message);
        }

        #endregion

        #region Deliver
        
        public void DeliverMessagesInMailbox()
        {
            int messagesToReceive = mailboxContentsAsIndexable.Count;

            for (int i = 0; i < messagesToReceive; i++)
            {
                var message = mailboxContentsAsIndexable[0];

                SendImmediately(message.Value);
                
                mailbox.Push(message);
            }
        }

        #endregion
        
        private void PushMessageToPool(IPoolElement<IMessage> message)
        {
            var messageType = message.Value.GetType();

            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;
	        
            messagePool.Push(message);
        }

        private void PushMessageToPool<TMessage>(IPoolElement<IMessage> message) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
	        
            if (!messageRepository.TryGet(
                    messageType,
                    out object messagePoolObject))
                throw new Exception($"[NonAllocMessageBus] INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {typeof(TMessage).ToString()}");

            INonAllocDecoratedPool<IMessage> messagePool = (INonAllocDecoratedPool<IMessage>)messagePoolObject;
	        
            messagePool.Push(message);
        }
        
        #endregion

        #region IMessageReceiverNonAlloc
        
        public void SubscribeTo<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Subscribe<TMessage>((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)subscription);
        }
        
        public void SubscribeTo(Type messageType, ISubscription subscription)
        {
            broadcaster.Subscribe(messageType, (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)subscription);
        }

        public void UnsubscribeFrom<TMessage>(ISubscription subscription) where TMessage : IMessage
        {
            broadcaster.Unsubscribe<TMessage>((ISubscriptionHandler<INonAllocSubscribableSingleArgGeneric<TMessage>, IInvokableSingleArgGeneric<TMessage>>)subscription);
        }
        
        public void UnsubscribeFrom(Type messageType, ISubscription subscription)
        {
            broadcaster.Unsubscribe(messageType, (ISubscriptionHandler<INonAllocSubscribableSingleArg, IInvokableSingleArg>)subscription);
        }
        
        #endregion
    }
}