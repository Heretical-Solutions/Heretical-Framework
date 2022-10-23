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

using HereticalSolutions.Assembly;
using HereticalSolutions.Assembly.Descriptors;
using HereticalSolutions.Assembly.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools.Services;

using Zenject;

public class RuntimeTesterAssembly : MonoBehaviour
{
	private const string KEY_DESCRIPTORS = "Descriptors";

	private const string KEY_PINGER = "Pinger";

	private const string KEY_COLLECTION_TYPE = "CollectionType";

	private const string KEY_POOL_PARENT = "PoolParent";

	private const string KEY_INIITIAL_ALLOCATION = "InitialAllocation";

	private const string KEY_ADDITIONAL_ALLOCATION = "AdditionalAllocation";

	[Inject(Id = "PoolBus")]
	MessageBus poolBus;

	[Inject(Id = "CurrentContainer")]
	DiContainer container;

	[Header("Settings")]

	[SerializeField]
	private string id;

	[SerializeField]
	private Transform poolParent;

	[SerializeField]
	private AddressVariantTimerDescriptor[] descriptors;

	[Space]

	[Header("Initial allocation")]

	[SerializeField]
	private AllocationCommandDescriptor initial;

	[Space]

	[Header("Additional allocation")]

	[SerializeField]
	private AllocationCommandDescriptor additional;


	private INonAllocDecoratedPool<GameObject> nonAllocPool;

	private WorldPositionArgument worldPositionArgumentNoTimer;

	private WorldPositionArgument worldPositionArgumentTimer;

	private AddressArgument addressArgumentNoTimer;

	private AddressArgument addressArgumentTimer;

	private TimerArgument timerArgument;

	private IPoolDecoratorArgument[] argumentsCache;

	private IPoolDecoratorArgument[] argumentsCacheWithTime;

	private Pinger pinger;

	private PoolElementResolutionService resolutionService;

	private int[][] addresses;

	void Start()
	{
		pinger = PubSubFactory.BuildPinger();


		nonAllocPool = BuildNonAllocPool(
			poolParent,
			descriptors,
			initial,
			additional,
			pinger);


		var poolRepository = RepositoryFactory.BuildDictionaryRepository<string, INonAllocDecoratedPool<GameObject>>();

		poolRepository.Add(id, nonAllocPool);

		resolutionService = new PoolElementResolutionService(
			poolBus,
			poolRepository);


		argumentsCache = new ArgumentBuilder()
			.Add<AddressArgument>(out addressArgumentNoTimer)
			.Add<WorldPositionArgument>(out worldPositionArgumentNoTimer)
			.Build();

		argumentsCacheWithTime = new ArgumentBuilder()
			.Add<AddressArgument>(out addressArgumentTimer)
			.Add<WorldPositionArgument>(out worldPositionArgumentTimer)
			.Add<TimerArgument>(out timerArgument)
			.Build();

		addresses = new int[descriptors.Length][];

		for (int i = 0; i < descriptors.Length; i++)
			addresses[i] = descriptors[i].Address.AddressToHashes();
	}

	private static INonAllocDecoratedPool<GameObject> BuildNonAllocPool(
		Transform poolParent,
		AddressVariantTimerDescriptor[] descriptors,
		AllocationCommandDescriptor initial,
		AllocationCommandDescriptor additional,
		Pinger pinger)
	{
		//Creating stuff required for assembly happens in top-to-bottom order
		//Assembly itself happens in bottom-to-top order
		IShop<INonAllocDecoratedPool<GameObject>>[] assemblyLine = 
			new AssemblyBuilder<INonAllocDecoratedPool<GameObject>>()
			//First, let's make the providables list to resolve them later after the final shop has yielded the final pool
			.Add(new ProvidableShop())
			//Then let's resolve the Address->Variant->Timer chain
			.Add(new AddressedPoolShop())
			.Add(new VariantPoolShop())
			.Add(new TimerPoolShop())
			//Then let's make the processors that shall operate on each new allocation
			.Add(new ProcessorsShop())
			//Then let's select a proper element container and define the container allocation delegate
			.Add(new AllocationDelegateShop())
			//Finally, game object pool
			.Add(new GameObjectPoolShop())
			.Build();

		AssemblyTicket<INonAllocDecoratedPool<GameObject>> ticket = TicketFactory.BuildTicket<INonAllocDecoratedPool<GameObject>>(assemblyLine);

		ticket.Arguments.Add(KEY_COLLECTION_TYPE, typeof(SupplyAndMergePool<GameObject>));
		ticket.Arguments.Add(KEY_POOL_PARENT, poolParent);
		ticket.Arguments.Add(KEY_DESCRIPTORS, descriptors);
		ticket.Arguments.Add(KEY_INIITIAL_ALLOCATION, initial);
		ticket.Arguments.Add(KEY_ADDITIONAL_ALLOCATION, additional);
		ticket.Arguments.Add(KEY_PINGER, pinger);

		return assemblyLine[0].Assemble(ticket, 0);
	}

	// Update is called once per frame
	void Update()
	{
		pinger.Ping();

		//UnityEngine.Debug.Log("[RuntimeTesterTimers] ---------------");

		bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

		if (!doSomething)
			return;

		int[] addressHashes = addresses[UnityEngine.Random.Range(0, descriptors.Length)];

		Vector3 newPosition = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

		bool rollForExtendedTime = UnityEngine.Random.Range(0f, 1f) < 0.1f;

		if (rollForExtendedTime)
		{
			addressArgumentTimer.AddressHashes = addressHashes;

			worldPositionArgumentTimer.Position = newPosition;

			timerArgument.Duration = 3f;

			var value = nonAllocPool.Pop(argumentsCacheWithTime);
		}
		else
		{
			addressArgumentNoTimer.AddressHashes = addressHashes;

			worldPositionArgumentNoTimer.Position = newPosition;

			var value = nonAllocPool.Pop(argumentsCache);
		}

		//Debug.Break();
	}
}
