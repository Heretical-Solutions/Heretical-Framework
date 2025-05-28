using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Messaging.Concurrent
{
	public class ConcurrentMessageBus
		: IMessageSender,
		  IMessageSubscribable
	{
		private readonly IPublisherSingleArg broadcaster;

		private readonly IReadOnlyRepository<Type, IPool<IMessage>> messagePoolRepository;

		private readonly Queue<IMessage> mailbox;

		private readonly ILogger logger;

		private readonly object mailboxLock;

		private readonly object poolLock;

		public ConcurrentMessageBus(
			BroadcasterWithRepository broadcaster,
			IReadOnlyRepository<Type, IPool<IMessage>> messagePoolRepository,
			Queue<IMessage> mailbox,
			object mailboxLock,
			object poolLock,
			ILogger logger)
		{
			this.broadcaster = broadcaster;

			this.messagePoolRepository = messagePoolRepository;

			this.mailbox = mailbox;

			this.mailboxLock = mailboxLock;

			this.poolLock = poolLock;

			this.logger = logger;
		}

		#region IMessageSender

		#region Pop

		public IMessageSender PopMessage<TMessage>(
			out TMessage message)
			where TMessage : IMessage
		{
			lock (poolLock)
			{
				if (!messagePoolRepository.TryGet(
					typeof(TMessage),
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(TMessage)}"));
				}

				var messageNonCasted = messagePool.Pop();

				message = (TMessage)messageNonCasted;
			}

			return this;
		}

		public IMessageSender PopMessage(
			Type messageType,
			out IMessage message)
		{
			lock (poolLock)
			{
				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(messageType)}"));
				}

				message = messagePool.Pop();
			}

			return this;
		}

		#endregion

		#region Write

		public IMessageSender Write<TMessage>(
			TMessage message,
			object[] args) where TMessage : IMessage
		{
			if (message == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID MESSAGE"));
			}

			message.Write(args);

			return this;
		}

		public IMessageSender Write(
			IMessage message,
			object[] args)
		{
			if (message == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID MESSAGE"));
			}

			message.Write(args);

			return this;
		}

		#endregion

		#region Put into mailbox

		public void PutIntoMailbox<TMessage>(
			TMessage message)
			where TMessage : IMessage
		{
			lock (mailboxLock)
			{
				mailbox.Enqueue(message);
			}
		}

		public void PutIntoMailbox(
			IMessage message)
		{
			lock (mailboxLock)
			{
				mailbox.Enqueue(message);
			}
		}

		public void SendImmediately<TMessage>(
			TMessage message)
			where TMessage : IMessage
		{
			broadcaster.Publish<TMessage>(
				message);

			PushMessageToPool<TMessage>(
				message);
		}

		public void SendImmediately(
			IMessage message)
		{
			broadcaster.Publish(
				message.GetType(),
				message);

			PushMessageToPool(
				message);
		}

		#endregion

		#region Deliver

		public void DeliverMessagesInMailbox()
		{
			int messagesToReceive = 0;

			lock (mailboxLock)
			{
				messagesToReceive = mailbox.Count;
			}

			for (int i = 0; i < messagesToReceive; i++)
			{
				IMessage message;

				lock (mailboxLock)
				{
					message = mailbox.Dequeue();
				}

				SendImmediately(message);
			}
		}

		#endregion

		private void PushMessageToPool<TMessage>(
			TMessage message)
			where TMessage : IMessage
		{
			lock (poolLock)
			{
				var messageType = typeof(TMessage);

				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {nameof(TMessage)}"));
				}

				messagePool.Push(message);
			}
		}

		private void PushMessageToPool(
			IMessage message)
		{
			lock (poolLock)
			{
				var messageType = message.GetType();

				if (!messagePoolRepository.TryGet(
					messageType,
					out var messagePool))
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"INVALID MESSAGE TYPE FOR PARTICULAR MESSAGE BUS: {messageType.Name}"));
				}

				messagePool.Push(message);
			}
		}

		#endregion

		#region IMessageReceiver

		public void SubscribeTo<TMessage>(
			Action<TMessage> receiverDelegate)
			where TMessage : IMessage
		{
			var broadcasterAsSubscribable = broadcaster as ISubscribableSingleArg;

			broadcasterAsSubscribable?.Subscribe<TMessage>(
				receiverDelegate);
		}

		public void SubscribeTo(
			Type messageType,
			object receiverDelegate)
		{
			var broadcasterAsSubscribable = broadcaster as ISubscribableSingleArg;

			broadcasterAsSubscribable?.Subscribe(
				messageType,
				receiverDelegate);
		}

		public void UnsubscribeFrom<TMessage>(
			Action<TMessage> receiverDelegate)
			where TMessage : IMessage
		{
			var broadcasterAsSubscribable = broadcaster as ISubscribableSingleArg;

			broadcasterAsSubscribable?.Unsubscribe<TMessage>(
				receiverDelegate);
		}

		public void UnsubscribeFrom(
			Type messageType,
			object receiverDelegate)
		{
			var broadcasterAsSubscribable = broadcaster as ISubscribableSingleArg;

			broadcasterAsSubscribable?.Unsubscribe(
				messageType,
				receiverDelegate);
		}

		#endregion
	}
}