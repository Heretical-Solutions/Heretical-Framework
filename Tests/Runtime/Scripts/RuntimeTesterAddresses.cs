using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Arguments;

using System.Collections.Generic;

using HereticalSolutions.Pools.AllocationProcessors;

using HereticalSolutions.Repositories;

using HereticalSolutions.Assembly.Descriptors;

public class RuntimeTesterAddresses : MonoBehaviour
{
    [Header("Settings")]

	[SerializeField]
	private string id;

    [SerializeField]
    private AddressedDescriptor[] addressedVariants;

	[SerializeField]
	private Transform poolParent;

	[Space]

	[Header("Initial allocation")]

	[SerializeField]
	private AllocationCommandDescriptor initial;

	[Space]

	[Header("Additional allocation")]

    [SerializeField]
    private AllocationCommandDescriptor additional;

    private INonAllocDecoratedPool<GameObject> nonAllocPool;

    private IndexedPackedArray<IPoolElement<GameObject>> activeElements;

    private WorldPositionArgument worldPositionArgument;

	private AddressArgument addressArgument;

    private IPoolDecoratorArgument[] argumentsCache;

    void Start()
    {
        nonAllocPool = BuildNonAllocPool(
			addressedVariants,
            poolParent,
            initial,
            additional);

        activeElements = CollectionFactory.BuildIndexedPackedArray<IPoolElement<GameObject>>(
			CollectionFactory.BuildPoolElementAllocationCommand<IPoolElement<GameObject>>(
                new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
                    Amount = 100
                },
				() => { return null; },
				CollectionFactory.BuildIndexedContainer));

        argumentsCache = new ArgumentBuilder()
            .Add<WorldPositionArgument>(out worldPositionArgument)
			.Add<AddressArgument>(out addressArgument)
            .Build();
    }

	public class AddressTreeNode
	{
		public int Level = 0;
		
		public int AddressHash = -1;

		public int[] FullAddressHashes = null;

		public AddressedDescriptor Descriptor;

		public List<AddressTreeNode> Children = new List<AddressTreeNode>();
	}

	private static INonAllocDecoratedPool<GameObject> BuildNonAllocPool(
		AddressedDescriptor[] variants,
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional)
	{
		List<IPoolProvidable<GameObject>> poolProvidables = new List<IPoolProvidable<GameObject>>();

		AddressTreeNode rootNode = Parse(variants);

        var result = BuildAddressedPool(
					rootNode,
					poolParent,
					initial,
					additional,
					poolProvidables);

		foreach (var poolProvidable in poolProvidables)
			poolProvidable.SetPool(result);

		return result;
	}

	private static AddressTreeNode Parse(AddressedDescriptor[] variants)
	{
		AddressTreeNode root = new AddressTreeNode
		{
			AddressHash = -1,

			Level = 0
		};

		foreach (var variant in variants)
		{
			Parse(
				root,
				variant);
		}

		return root;
	}

	private static void Parse(
		AddressTreeNode root,
		AddressedDescriptor descriptor)
	{
		int[] addressHashes = descriptor.Address.AddressToHashes();

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
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional,
		List<IPoolProvidable<GameObject>> poolProvidables)
    {
		int level = node.Level;

		Dictionary<int, INonAllocDecoratedPool<GameObject>> database = new Dictionary<int, INonAllocDecoratedPool<GameObject>>();

		foreach (var child in node.Children)
		{
			INonAllocDecoratedPool<GameObject> poolByHash;

			if (child.Children.Count != 0)
			{
				poolByHash = BuildAddressedPool(
					child,
					poolParent,
					initial,
					additional,
					poolProvidables);
			}
			else
			{
				poolByHash = BuildLeafPool(
					child.FullAddressHashes,
					child.Descriptor.Prefab,
					poolParent,
					initial,
					additional,
					poolProvidables);
			}

			database.Add(child.AddressHash, poolByHash);
		}

		var repository = new DictionaryRepository<int, INonAllocDecoratedPool<GameObject>>(database);

		var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

		builder
			.Add(new CompositePoolWithAddresses<GameObject>(repository, level));

		return builder.CurrentWrapper;
    }

	private static INonAllocDecoratedPool<GameObject> BuildLeafPool(
		int[] addressHashes,
		GameObject prefab,
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional,
		List<IPoolProvidable<GameObject>> poolProvidables)
	{
		NewGameObjectsPusher pusher = new NewGameObjectsPusher();

		PoolElementBehaviourInitializer initializer = new PoolElementBehaviourInitializer();

		NameByStringAndIndex namer = new NameByStringAndIndex(prefab.name);

		CompositeGameObjectAllocationProcessor processor = new CompositeGameObjectAllocationProcessor(
			new Stack<IPoolElement<GameObject>>(),
			new IAllocationProcessor[]
			{
				initializer,
				pusher,
				namer
			});

		INonAllocPool<GameObject> packedArrayPool = PoolFactory.BuildGameObjectPool(
			new BuildNonAllocGameObjectPoolCommand
			{
				CollectionType = typeof(PackedArrayPool<GameObject>),
				Prefab = prefab,
				InitialAllocation = initial,
				AdditionalAllocation = additional,
				ContainerAllocationDelegate = PoolFactory.BuildPoolElementWithAddressAndVariantAllocationDelegate<GameObject>(
					processor,
					addressHashes,
					-1)
			});

		var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

		builder
			.Add(new NonAllocWrapperPool<GameObject>(packedArrayPool))
			.Add(new NonAllocGameObjectPool(builder.CurrentWrapper, poolParent));

		poolProvidables.Add(processor);

		return builder.CurrentWrapper;
	}

    // Update is called once per frame
    void Update()
    {
        bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

        if (!doSomething)
            return;

        bool push = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        if (push)
        {
            if (activeElements.Count == 0)
                return;

            var randomIndex = UnityEngine.Random.Range(0, activeElements.Count);

            var activeElement = activeElements[randomIndex];

            nonAllocPool.Push(activeElement.Value);

            activeElements.Push(activeElement);
        }
        else
        {
            worldPositionArgument.Position = new Vector3(
                UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

			addressArgument.AddressHashes =
				addressedVariants[UnityEngine.Random.Range(0, addressedVariants.Length)]
				.Address
				.AddressToHashes();

            var value = nonAllocPool.Pop(argumentsCache);

            var activeElement = activeElements.Pop();

            activeElement.Value = value;
        }

        //Debug.Break();
    }
}
