# INonAllocSubscribable

Represents a subscribable object that allows non-allocating subscriptions. Contains methods that are not dependent on the type or the amount of arguments the subscribers can pass to the publisher. For interfaces that are dependent on the amount of arguments, see [`INonAllocSubscribable`](INonAllocSubscribable.md), [`INonAllocSubscribableSingleArg`](INonAllocSubscribableSingleArg.md), [`INonAllocSubscribable`](INonAllocSubscribableSingleArgGeneric.md), and [`INonAllocSubscribable`](INonAllocSubscribable.md). For the allocating version, see [`ISubscribable`](ISubscribable.md).

## Methods

Method | Description
--- | ---
`IEnumerable<INonAllocSubscription> AllSubscriptions { get; }` | Gets all the current subscriptions to this object
`void UnsubscribeAll()` | Unsubscribes all objects from this object

## Using INonAllocSubscribable

### List all subscriptions

```csharp
INonAllocSubscribable foo;

IEnumerable<INonAllocSubscription> allSubscriptions = foo.AllSubscriptions;
```

### Cancel all subscriptions

```csharp
INonAllocSubscribable foo;

foo.UnsubscribeAll();
```

## Creating INonAllocSubscribable

```csharp
//Logger resolver is needed for the pool to log errors
ILoggerResolver loggerResolver;

//Create a non alloc subscribable with no arguments
INonAllocSubscribable foo = DelegatesFactory.BuildBuildNonAllocPinger(loggerResolver);

//Create a non alloc subscribable with a single generic argument
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocBroadcasterGeneric<T>(loggerResolver);

//An object repository is needed for the broadcaster to select the concrete broadcaster for a given argument type
IReadOnlyInstanceRepository repository;

//Create a non alloc subscribable with a single argument
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocBroadcasterWithRepository(
    repository,
    loggerResolver);

//Create a non alloc subscribable with multiple arguments
INonAllocSubscribable foo = DelegatesFactory.BuildNonAllocBroadcasterMultipleArgs(loggerResolver);
```

## Implementing INonAllocSubscribable

```csharp
private IIndexable<IPoolElement<INonAllocSubscription>> subscriptionsAsIndexable;

#region INonAllocSubscribable

public IEnumerable<INonAllocSubscription> AllSubscriptions
{
	get
	{
		INonAllocSubscription[] allSubscriptions = new INonAllocSubscription[subscriptionsAsIndexable.Count];

		for (int i = 0; i < allSubscriptions.Length; i++)
			allSubscriptions[i] = subscriptionsAsIndexable[i].Value;

		return allSubscriptions;
	}
}

public void UnsubscribeAll()
{
	while (subscriptionsAsIndexable.Count > 0)
	{
		var subscription = (INonAllocSubscriptionHandler<
			INonAllocSubscribable,
			IInvokableSingleArgGeneric<T>>)
			subscriptionsAsIndexable[0].Value;

		Unsubscribe(subscription);
	}
}

#endregion
```
