using Zenject;
using UnityEngine;
using UniRx;

using HereticalSolutions.Messaging;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Factories;
using HereticalSolutions.Allocations;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools.Messages;

using System;
using System.Collections.Generic;

namespace DI.SceneInstallers
{
	public class MessageBusInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<MessageBus>()
				.WithId("PoolBus")
				.FromInstance(
					new MessageBus(
						new MessageBroker(),
						BuildMessageRepository(
								new[]
								{
									typeof(ResolvePoolElementRequestMessage)
								}),
						new Queue<IMessage>()))
				.AsCached();

			Container.Bind<DiContainer>().WithId("CurrentContainer").FromInstance(Container).AsCached();
		}

		private IRepository<Type, StackPool<IMessage>> BuildMessageRepository(Type[] messageTypes)
		{
			var result = new Dictionary<Type, StackPool<IMessage>>();

			foreach (var messageType in messageTypes)
			{
				result.Add(
					messageType,
					CollectionFactory.BuildStackPool<IMessage>(
						new AllocationCommand<IMessage>
						{
							Descriptor = new AllocationCommandDescriptor
							{
								Rule = EAllocationAmountRule.ADD_ONE,
								Amount = 1
							},
							AllocationDelegate = () => (IMessage)Activator.CreateInstance(messageType),
						},
						new AllocationCommand<IMessage>
						{
							Descriptor = new AllocationCommandDescriptor
							{
								Rule = EAllocationAmountRule.ADD_ONE,
								Amount = 1
							},
							AllocationDelegate = () =>
							{
								Debug.Log($"[MessageBrokerInstaller] RESIZING POOL: {{ {messageType.ToString()} }}");

								return (IMessage)Activator.CreateInstance(messageType);
							},
						}));
			}

			return BuildDictionaryRepository(result);
		}

		private DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
			Dictionary<TKey, TValue> database)
		{
			return new DictionaryRepository<TKey, TValue>(database);
		}
	}
}