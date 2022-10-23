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

public class RuntimeTester : MonoBehaviour
{
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

	[Space]

	[Header("Debug")]

	[SerializeField]
	private bool pushedNow;

    private INonAllocDecoratedPool<GameObject> nonAllocPool;

    private IndexedPackedArray<IPoolElement<GameObject>> activeElements;

    private WorldPositionArgument argument;

    private IPoolDecoratorArgument[] argumentsCache;

    void Start()
    {
        nonAllocPool = BuildNonAllocPool(
            id,
            prefab,
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
            .Add<WorldPositionArgument>(out argument)
            .Build();
    }

    private static INonAllocDecoratedPool<GameObject> BuildNonAllocPool(
		string ID,
        GameObject prefab,
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional)
    {
		NewGameObjectsPusher pusher = new NewGameObjectsPusher();

		PoolElementBehaviourInitializer initializer = new PoolElementBehaviourInitializer();

		CompositeGameObjectAllocationProcessor processor = new CompositeGameObjectAllocationProcessor(
			new Stack<IPoolElement<GameObject>>(),
			new IAllocationProcessor[]
			{
                initializer,
                pusher
			});

        INonAllocPool<GameObject> packedArrayPool = PoolFactory.BuildGameObjectPool(
            new BuildNonAllocGameObjectPoolCommand
            {
                CollectionType = typeof(PackedArrayPool<GameObject>),
                Prefab = prefab,
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

    // Update is called once per frame
    void Update()
    {
        bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

        if (!doSomething)
            return;

        bool push = UnityEngine.Random.Range(0f, 1f) < 0.5f;

		pushedNow = push;

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

        //Debug.Break();
    }
}
