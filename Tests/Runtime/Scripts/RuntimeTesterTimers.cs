using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Arguments;

using System.Collections.Generic;

using HereticalSolutions.Pools.AllocationProcessors;

using HereticalSolutions.Messaging;

using HereticalSolutions.Timers;

using HereticalSolutions.Messaging.Factories;

public class RuntimeTesterTimers : MonoBehaviour
{
    [Header("Settings")]

	[SerializeField]
	private string id;

    [SerializeField]
    private GameObject prefab;

	[SerializeField]
	private Transform poolParent;

	[SerializeField]
	private float defaultDuration;

	[Space]

	[Header("Initial allocation")]

	[SerializeField]
	private AllocationCommandDescriptor initial;

	[Space]

	[Header("Additional allocation")]

    [SerializeField]
    private AllocationCommandDescriptor additional;


    private INonAllocDecoratedPool<GameObject> nonAllocPool;

    private WorldPositionArgument argument;

	private WorldPositionArgument argument2;

    private TimerArgument argument3;

    private IPoolDecoratorArgument[] argumentsCache;

	private IPoolDecoratorArgument[] argumentsCacheWithTime;

    private Pinger pinger;

    void Start()
    {
        pinger = PubSubFactory.BuildPinger();

        nonAllocPool = BuildNonAllocPool(
            id,
            prefab,
            poolParent,
            initial,
            additional,
            defaultDuration,
            pinger);

        argumentsCache = new ArgumentBuilder()
            .Add<WorldPositionArgument>(out argument)
            .Build();

		argumentsCacheWithTime = new ArgumentBuilder()
            .Add<WorldPositionArgument>(out argument2)
			.Add<TimerArgument>(out argument3)
            .Build();
    }

    private static INonAllocDecoratedPool<GameObject> BuildNonAllocPool(
		string ID,
        GameObject prefab,
		Transform poolParent,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional,
        float defaultDuration,
        Pinger pinger)
    {
        List<IPoolProvidable<GameObject>> poolProvidables = new List<IPoolProvidable<GameObject>>();
        
		NewGameObjectsPusher pusher = new NewGameObjectsPusher();

		NameByStringAndIndex namer = new NameByStringAndIndex(prefab.name);

		CompositeGameObjectAllocationProcessor processor = new CompositeGameObjectAllocationProcessor(
			new Stack<IPoolElement<GameObject>>(),
			new IAllocationProcessor[]
			{
                pusher,
                namer
			});

        poolProvidables.Add(processor);

        INonAllocPool<GameObject> packedArrayPool = PoolFactory.BuildGameObjectPool(
            new BuildNonAllocGameObjectPoolCommand
            {
                CollectionType = typeof(PackedArrayPool<GameObject>),
                Prefab = prefab,
                InitialAllocation = initial,
                AdditionalAllocation = additional,
                ContainerAllocationDelegate = PoolFactory.BuildPoolElementWithTimerAllocationDelegate<GameObject>(
                    processor,
                    () =>
                    {
                        return new Timer(
                            defaultDuration,
                            pinger);
                    },
                    new int[0],
                    -1)
            });

		var builder = new NonAllocDecoratedPoolBuilder<GameObject>();

		builder
			.Add(new NonAllocWrapperPool<GameObject>(packedArrayPool))
			.Add(new NonAllocGameObjectPool(builder.CurrentWrapper, poolParent))
			.Add(new NonAllocPoolWithTimer(builder.CurrentWrapper), out var poolWithTimer);

		poolProvidables.Add((IPoolProvidable<GameObject>)poolWithTimer);

        foreach (var poolProvidable in poolProvidables)
            poolProvidable.SetPool(builder.CurrentWrapper);

        return builder.CurrentWrapper;
    }

    // Update is called once per frame
    void Update()
    {
        pinger.Ping();

        //UnityEngine.Debug.Log("[RuntimeTesterTimers] ---------------");

        bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

        if (!doSomething)
            return;

		bool rollForExtendedTime = UnityEngine.Random.Range(0f, 1f) < 0.1f;

        if (rollForExtendedTime)
        {
			argument2.Position = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

            argument3.Duration = 3f;

			var value = nonAllocPool.Pop(argumentsCacheWithTime);
        }
        else
        {
            argument.Position = new Vector3(
                UnityEngine.Random.Range(-5f, 5f),
                UnityEngine.Random.Range(-5f, 5f),
                UnityEngine.Random.Range(-5f, 5f));

            var value = nonAllocPool.Pop(argumentsCache);
        }
        
        //Debug.Break();
    }
}
