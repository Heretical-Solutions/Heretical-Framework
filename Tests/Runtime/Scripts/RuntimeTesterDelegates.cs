using UnityEngine;

using HereticalSolutions.Collections;
using HereticalSolutions.Collections.Managed;
using HereticalSolutions.Collections.Factories;

using HereticalSolutions.Allocations;

using HereticalSolutions.Messaging;
using HereticalSolutions.Messaging.Factories;

public class RuntimeTesterDelegates : MonoBehaviour
{
	private Pinger pinger;

	private IndexedPackedArray<IPoolElement<PingerSubscription>> activeDelegates;

    private StackPool<PingerSubscription> freeSubscriptions;

	void Start()
	{
        int currentUUID = -1;

		pinger = PubSubFactory.BuildPinger();

		activeDelegates = CollectionFactory.BuildIndexedPackedArray<IPoolElement<PingerSubscription>>(
			CollectionFactory.BuildPoolElementAllocationCommand<IPoolElement<PingerSubscription>>(
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
					Amount = 100
				},
				() => { return null; },
				CollectionFactory.BuildIndexedContainer));

        freeSubscriptions = CollectionFactory.BuildStackPool<PingerSubscription>(
            new AllocationCommand<PingerSubscription>
            {
                Descriptor = new AllocationCommandDescriptor
                {
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
					Amount = 100
                },
                AllocationDelegate = () => 
                {
					currentUUID++;

                    int uuidClosure = currentUUID;

                    return new PingerSubscription(() => 
                        {
                            TestPing(uuidClosure);
                        });
                }
            },
			new AllocationCommand<PingerSubscription>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_ONE
				},
				AllocationDelegate = () =>
				{
					currentUUID++;

					int uuidClosure = currentUUID;

					return new PingerSubscription(() =>
						{
							TestPing(uuidClosure);
						});
				}
			});
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log("[RuntimeTesterDelegates] --------------------------------");

		//Debug.Log($"[RuntimeTesterDelegates] Active delegates: {{ {activeDelegates.Count} }}");

        pinger.Ping();

		bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.3f;

		if (!doSomething)
			return;

		bool push = UnityEngine.Random.Range(0f, 1f) < 0.5f;

		if (push)
		{
			if (activeDelegates.Count == 0)
				return;

			var randomIndex = UnityEngine.Random.Range(0, activeDelegates.Count);

			var activeDelegate = activeDelegates[randomIndex];


			freeSubscriptions.Push(activeDelegate.Value.Value);

			pinger.Unsubscribe(activeDelegate.Value);

			activeDelegates.Push(activeDelegate);
		}
		else
		{
            var subscription = freeSubscriptions.Pop();

			var subscriptionElement = pinger.Subscribe(subscription);


			var activeElement = activeDelegates.Pop();

            subscriptionElement.Value = subscription;

			activeElement.Value = subscriptionElement;
		}

		//Debug.Break();
	}

    private void TestPing(int uuid)
    {
		//Debug.Log($"[RuntimeTesterDelegates] Ping {{ {uuid} }}");
    }
}