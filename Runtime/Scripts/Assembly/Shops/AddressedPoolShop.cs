using System.Collections.Generic;

using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Assembly.Descriptors;
using HereticalSolutions.Assembly.Factories;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Assembly
{
	public class AddressedPoolShop : IShop<INonAllocDecoratedPool<GameObject>>
	{
		private const string KEY_DESCRIPTOR = "Descriptor";

		private const string KEY_DESCRIPTORS = "Descriptors";

		private const string KEY_PREFAB = "Prefab";

		private const string KEY_ADDRESS_HASHES = "AddressHashes";


		public INonAllocDecoratedPool<GameObject> Assemble(
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int level)
		{
			//Retrieve arguments
			var descriptors = ticket.Arguments.Get(KEY_DESCRIPTORS);


			//Next shop
			var nextShop = ticket.AssemblyLine[++level];


			//Return
			AddressTreeNode rootNode = Parse(descriptors);

			var result = BuildAddressedPool(
				rootNode,
				nextShop,
				ticket,
				level);

			return result;
		}

		private class AddressTreeNode
		{
			public int Level = 0;

			public int AddressHash = -1;

			public int[] FullAddressHashes = null;

			public object Descriptor;

			public List<AddressTreeNode> Children = new List<AddressTreeNode>();
		}

		private static AddressTreeNode Parse(object descriptors)
		{
			AddressTreeNode root = new AddressTreeNode
			{
				AddressHash = -1,

				Level = 0
			};

			if (descriptors is AddressedDescriptor[])
			{
				var addressDescriptors = (AddressedDescriptor[])descriptors;

				foreach (var descriptor in addressDescriptors)
				{
					string address = descriptor.Address;

					Parse(
						root,
						address,
						descriptor);
				}
			}
			else if (descriptors is AddressTimerDescriptor[])
			{
				var addressTimerDescriptors = (AddressTimerDescriptor[])descriptors;

				foreach (var descriptor in addressTimerDescriptors)
				{
					string address = descriptor.Address;

					Parse(
						root,
						address,
						descriptor);
				}
			}
			else if (descriptors is AddressVariantDescriptor[])
			{
				var addressVariantDescriptors = (AddressVariantDescriptor[])descriptors;

				foreach (var descriptor in addressVariantDescriptors)
				{
					string address = descriptor.Address;

					Parse(
						root,
						address,
						descriptor);
				}
			}
			else if (descriptors is AddressVariantTimerDescriptor[])
			{
				var addressVariantTimerDescriptors = (AddressVariantTimerDescriptor[])descriptors;

				foreach (var descriptor in addressVariantTimerDescriptors)
				{
					string address = descriptor.Address;

					Parse(
						root,
						address,
						descriptor);
				}
			}

			return root;
		}

		private static void Parse(
			AddressTreeNode root,
			string address,
			object descriptor)
		{
			int[] addressHashes = address.AddressToHashes();

			AddressTreeNode currentNode = root;

			for (int i = 0; i < addressHashes.Length; i++)
			{
				bool success = false;

				for (int j = 0; j < currentNode.Children.Count; j++)
				{
					if (currentNode.Children[j].AddressHash == addressHashes[i])
					{
						currentNode = currentNode.Children[j];

						success = true;

						break;
					}
				}

				if (!success)
				{
					AddressTreeNode child = new AddressTreeNode
					{
						AddressHash = addressHashes[i],

						Level = i + 1
					};

					currentNode.Children.Add(child);

					currentNode = child;
				}
			}

			currentNode.FullAddressHashes = addressHashes;

			currentNode.Descriptor = descriptor;
		}

		private static INonAllocDecoratedPool<GameObject> BuildAddressedPool(
			AddressTreeNode node,
			IShop<INonAllocDecoratedPool<GameObject>> nextShop,
			AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket,
			int assemblyLevel)
		{
			int level = node.Level;

			Dictionary<int, INonAllocDecoratedPool<GameObject>> database = new Dictionary<int, INonAllocDecoratedPool<GameObject>>();

			foreach (var child in node.Children)
			{
				INonAllocDecoratedPool<GameObject> poolByHash;

				if (child.Children.Count != 0)
				{
					//You may find yourself,
					//Parsing "ENEMY/SOLDIER"
					//And you may find yourself,
					//Parsing "ENEMY/SOLDIER/RIFLEMAN"
					//And you may find yourself,
					//Resolving prefab element behaviour
					//With "ENEMY/SOLDIER" address
					//And reach the depth level 3
					//And catching an exception
					//And you may ask yourself,
					//How did I get here?

					//If this node (i.e. "ENEMY/SOLDIER") was parsed as a terminal part of some address then it will have a descriptor assigned to it
					//All we have to do now is to give it a "self" child with hash 0 to refer to the prefab that was parsed with this address
					if (child.Descriptor != null)
					{
						AddressTreeNode self = new AddressTreeNode
						{
							AddressHash = 0,

							Level = child.Level + 1,

							FullAddressHashes = child.FullAddressHashes,

							Descriptor = child.Descriptor

						};

						child.Children.Add(self);
					}

					poolByHash = BuildAddressedPool(
						child,
						nextShop,
						ticket,
						assemblyLevel);
				}
				else
				{
					var nextShopTicket = TicketFactory.BuildNextShopTicket<INonAllocDecoratedPool<GameObject>>(ticket);

					nextShopTicket.Arguments.Add(KEY_ADDRESS_HASHES, child.FullAddressHashes);

					if (child.Descriptor is AddressedDescriptor)
					{
						nextShopTicket.Arguments.Remove(KEY_DESCRIPTORS);

						nextShopTicket.Arguments.Add(KEY_PREFAB, ((AddressedDescriptor)child.Descriptor).Prefab);
					}

					if (child.Descriptor is AddressTimerDescriptor)
					{
						nextShopTicket.Arguments.Remove(KEY_DESCRIPTORS);

						nextShopTicket.Arguments.Add(KEY_DESCRIPTOR, child.Descriptor);
					}

					if (child.Descriptor is AddressVariantDescriptor)
					{
						nextShopTicket.Arguments.Update(KEY_DESCRIPTORS, ((AddressVariantDescriptor)child.Descriptor).Variants);
					}

					if (child.Descriptor is AddressVariantTimerDescriptor)
					{
						nextShopTicket.Arguments.Update(KEY_DESCRIPTORS, ((AddressVariantTimerDescriptor)child.Descriptor).Variants);
					}

					poolByHash = nextShop.Assemble(nextShopTicket, assemblyLevel);
				}

				database.Add(child.AddressHash, poolByHash);
			}

			var repository = new DictionaryRepository<int, INonAllocDecoratedPool<GameObject>>(database);

			var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

			builder
				.Add(new CompositePoolWithAddresses<GameObject>(repository, level));

			return builder.CurrentWrapper;
		}
	}
}