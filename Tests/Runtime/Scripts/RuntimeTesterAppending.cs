using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.AllocationProcessors;

using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools.Services;

using HereticalSolutions.Messaging;

using Zenject;

public class RuntimeTesterAppending : MonoBehaviour
{
	[Inject(Id = "PoolBus")]
	MessageBus poolBus;

	[Inject(Id = "CurrentContainer")]
	DiContainer container;

	[Header("Settings")]

	[SerializeField]
	private string id;

	[SerializeField]
	private GameObject prefab;

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

	private WorldPositionArgument argument;

	private IPoolDecoratorArgument[] argumentsCache;

	private PoolElementResolutionService resolutionService;

	void Start()
	{
		activeElements = CollectionFactory.BuildIndexedPackedArray<IPoolElement<GameObject>>(
			CollectionFactory.BuildPoolElementAllocationCommand<IPoolElement<GameObject>>(
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
					Amount = 100
				},
				() => { return null; },
				CollectionFactory.BuildIndexedContainer));

		nonAllocPool = BuildNonAllocPool(
			id,
			prefab,
			container,
			poolParent,
			initial,
			additional,

			//DEBUG
			activeElements);

		argumentsCache = new ArgumentBuilder()
			.Add<WorldPositionArgument>(out argument)
			.Build();

		var poolRepository = RepositoryFactory.BuildDictionaryRepository<string, INonAllocDecoratedPool<GameObject>>();

		poolRepository.Add(id, nonAllocPool);

		resolutionService = new PoolElementResolutionService(
			poolBus,
			poolRepository);
	}

	public class AddToActiveElements : IAllocationProcessor
	{
		IndexedPackedArray<IPoolElement<GameObject>> activeElements;

		public AddToActiveElements(IndexedPackedArray<IPoolElement<GameObject>> activeElements)
		{
			this.activeElements = activeElements;
		}

		public void Process(
			INonAllocDecoratedPool<GameObject> poolWrapper,
			IPoolElement<GameObject> currentElement)
		{
			if (currentElement.Value == null)
				return;

			if (((IIndexed)currentElement).Index != -1)
			{
				var activeElement = activeElements.Pop();

				activeElement.Value = currentElement;
			}
		}
	}

	private static INonAllocDecoratedPool<GameObject> BuildNonAllocPool(
		string ID,
		GameObject prefab,
		DiContainer container,
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional,


		//DEBUG
		IndexedPackedArray<IPoolElement<GameObject>> activeElements
		)
	{
		NewGameObjectsPusher pusher = new NewGameObjectsPusher();

		PoolElementBehaviourInitializer initializer = new PoolElementBehaviourInitializer();

		CompositeGameObjectAllocationProcessor processor = new CompositeGameObjectAllocationProcessor(
			new Stack<IPoolElement<GameObject>>(),
			new IAllocationProcessor[]
			{
				initializer,
				pusher,

				//DEBUG
				new AddToActiveElements(activeElements)
			});

		INonAllocPool<GameObject> packedArrayPool = PoolFactory.BuildGameObjectPool(
			new BuildNonAllocGameObjectPoolCommand
			{
				CollectionType = typeof(SupplyAndMergePool<GameObject>),
				Prefab = prefab,
				Container = container,
				InitialAllocation = initial,
				AdditionalAllocation = additional,
				ContainerAllocationDelegate = PoolFactory.BuildValueAssignedNotifyingPoolElementAllocationDelegate<GameObject>(processor)
			});

		var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

		builder
			.Add(new NonAllocWrapperPool<GameObject>(packedArrayPool))
			.Add(new NonAllocGameObjectPool(builder.CurrentWrapper, poolParent))
			.Add(new NonAllocPrefabInstancePool(builder.CurrentWrapper, prefab))
			.Add(new NonAllocPoolWithID<GameObject>(builder.CurrentWrapper, ID));

		processor.SetPool(builder.CurrentWrapper);

		return builder.CurrentWrapper;
	}

	private bool guaranteedPushClaimed = false;

	// Update is called once per frame
	void Update()
	{
		bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

		if (!doSomething)
			return;

		bool push = UnityEngine.Random.Range(0f, 1f) < 0.5f;

		if (!guaranteedPushClaimed)
		{
			push = true;

			guaranteedPushClaimed = true;
		}

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
			argument.Position = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

			var value = nonAllocPool.Pop(argumentsCache);

			var activeElement = activeElements.Pop();

			activeElement.Value = value;
		}
	}
}
