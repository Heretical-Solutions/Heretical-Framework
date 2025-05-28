# Messaging Module

## Overview

The Messaging module provides a comprehensive event bus and message passing system designed for loose coupling and high-performance communication between game systems. It offers multiple variants optimized for different performance characteristics and threading requirements.

## Module Variants

### Messaging (Base)
- **Location**: `Assets/Scripts/Framework/Messaging/`
- **Purpose**: Standard message bus with full feature set
- **Use Cases**: General-purpose inter-system communication, event handling
- **Performance**: Allocating, full-featured implementation

### Messaging.Concurrent
- **Location**: `Assets/Scripts/Framework/Messaging.Concurrent/`
- **Purpose**: Thread-safe message passing for multi-threaded environments
- **Use Cases**: Multi-threaded game engines, server applications, concurrent system communication
- **Performance**: Thread-safe with appropriate synchronization

### Messaging.NonAlloc
- **Location**: `Assets/Scripts/Framework/Messaging.NonAlloc/`
- **Purpose**: Zero-allocation messaging for performance-critical scenarios
- **Use Cases**: Game loops, real-time systems, mobile optimization
- **Performance**: No garbage generation after initialization

### Messaging.NonAlloc.Concurrent
- **Location**: `Assets/Scripts/Framework/Messaging.NonAlloc.Concurrent/`
- **Purpose**: Thread-safe, zero-allocation messaging
- **Use Cases**: High-performance multi-threaded messaging
- **Performance**: Optimal for concurrent, garbage-sensitive scenarios

## Core Concepts

### Message Types
The messaging system supports various message categories:
- **Commands**: Request actions to be performed
- **Events**: Notify about state changes or occurrences
- **Queries**: Request information from systems
- **Responses**: Reply to queries or commands

### Message Routing
- **Topic-Based**: Messages routed by topic/type
- **Type-Safe**: Compile-time type checking for message handlers
- **Subscription-Based**: Publishers and subscribers loosely coupled
- **Filtering**: Advanced message filtering capabilities

## Core Interfaces

### Message Sender
```csharp
public interface IMessageSender
{
    IMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : class, IMessage;
    IMessageSender Write<TMessage>(TMessage message, params object[] args) where TMessage : class, IMessage;
    IMessageSender SendImmediately<TMessage>(TMessage message) where TMessage : class, IMessage;
}
```

### Message Receiver
```csharp
public interface IMessageSubscribable
{
    void SubscribeTo<TMessage>(Action<TMessage> handler) where TMessage : class, IMessage;
    void UnsubscribeTo<TMessage>(Action<TMessage> handler) where TMessage : class, IMessage;
}
```

### Message Interface
```csharp
public interface IMessage
{
    void Write(params object[] args);
}
```

### Advanced Interfaces

#### Message Router
```csharp
public interface IMessageRouter<TMessage> where TMessage : class
{
    void Route(TMessage message);
    void Subscribe(IMessageHandler<TMessage> handler);
    void Unsubscribe(IMessageHandler<TMessage> handler);
}
```

#### Message Handler
```csharp
public interface IMessageHandler<TMessage> where TMessage : class
{
    void Handle(TMessage message);
    bool CanHandle(TMessage message);
}
```

#### Message Filter
```csharp
public interface IMessageFilter<TMessage> where TMessage : class
{
    bool ShouldProcess(TMessage message);
}
```

### NonAlloc Interfaces\n\n#### NonAlloc Subscription Interface\n```csharp\npublic interface INonAllocSubscription\n{\n    bool Active { get; set; }\n}\n```\n\n**Purpose**: Manages non-allocating subscription state for performance-critical scenarios\n\n**Properties**:\n- **`Active { get; set; }`**\n  - **Purpose**: Controls whether the subscription is currently active without deallocating it\n  - **Get**: Returns true if subscription will receive messages, false if dormant\n  - **Set**: Enables/disables subscription without memory allocation\n  - **Use Cases**: Temporary subscription disabling, conditional message processing\n  - **Performance**: Zero allocation alternative to unsubscribe/resubscribe cycles\n  - **Thread Safety**: Implementation-dependent; check specific variant documentation\n  - **State Management**: Subscription remains in memory when inactive, ready for reactivation\n\n#### NonAlloc Message Sender/Receiver\n```csharp\npublic interface INonAllocMessageSender\n{\n    INonAllocMessageSender PopMessage<TMessage>(out TMessage message) where TMessage : class, IMessage;\n    INonAllocMessageSender Write<TMessage>(TMessage message, params object[] args) where TMessage : class, IMessage;\n    INonAllocMessageSender SendImmediately<TMessage>(TMessage message) where TMessage : class, IMessage;\n}\n\npublic interface INonAllocMessageSubscribable\n{\n    void SubscribeTo<TMessage>(INonAllocSubscription subscription) where TMessage : class, IMessage;\n    void UnsubscribeTo<TMessage>(INonAllocSubscription subscription) where TMessage : class, IMessage;\n}\n```\n\n**INonAllocMessageSender Purpose**: Non-allocating version of message sending with pre-pooled message instances\n- Uses pre-allocated message pools to avoid garbage collection\n- Message instances are reused after sending\n- Same fluent API pattern as standard message sender\n\n**INonAllocMessageSubscribable Purpose**: Non-allocating subscription management using subscription objects\n- No delegate allocation during subscription\n- Subscription objects can be reused by toggling Active property\n- Optimized for high-frequency subscribe/unsubscribe patterns\n\n## Message Bus Architecture

### Standard Message Bus
- **Storage**: Dictionary-based handler storage
- **Threading**: Single-threaded access
- **Memory**: Standard heap allocation
- **Performance**: Good general-purpose performance

### Concurrent Message Bus
- **Storage**: Thread-safe collections
- **Threading**: Lock-free or fine-grained locking
- **Memory**: Concurrent-optimized allocation
- **Performance**: High throughput in multi-threaded scenarios

### NonAlloc Message Bus
- **Storage**: Pre-allocated handler pools
- **Threading**: Single-threaded optimization
- **Memory**: Zero allocation after initialization
- **Performance**: Optimal for tight game loops

## Publishers and Subscribers

### Publisher Pattern
```csharp
public interface IPublisher<TMessage> where TMessage : class
{
    void Publish(TMessage message);
    void Subscribe(ISubscriber<TMessage> subscriber);
    void Unsubscribe(ISubscriber<TMessage> subscriber);
}
```

### Subscriber Pattern
```csharp
public interface ISubscriber<TMessage> where TMessage : class
{
    void OnMessage(TMessage message);
}
```

### Publisher Builder
```csharp
public interface IPublisherBuilder<TMessage> where TMessage : class
{
    IPublisherBuilder<TMessage> WithFilter(IMessageFilter<TMessage> filter);
    IPublisherBuilder<TMessage> WithRouter(IMessageRouter<TMessage> router);
    IPublisher<TMessage> Build();
}
```

## Message Lifecycle

### Message Flow
1. **Creation**: Message object instantiated
2. **Publishing**: Sent through message bus
3. **Routing**: Distributed to appropriate handlers
4. **Filtering**: Applied filters determine processing
5. **Handling**: Message processed by subscribers
6. **Cleanup**: Message disposed (if applicable)

### Handler Management
- **Dynamic Subscription**: Runtime handler addition/removal
- **Priority Handling**: Ordered message processing
- **Exception Isolation**: Handler failures don't affect others
- **Lifecycle Integration**: Automatic cleanup on object disposal

## Performance Variants

### Allocation Strategies

#### Standard Variant
```csharp
// Message bus setup
var builder = new MessageBusBuilder(repositoryFactory, configurableStackPoolFactory, broadcasterWithRepositoryBuilder, messagingFactory);
builder.New();
builder.AddMessageType<SampleMessage>();
var messageBus = builder.BuildMessageBus();

// Sending messages
messageBusAsSender.PopMessage<SampleMessage>(out var message)
    .Write<SampleMessage>(message, messageArgs)
    .SendImmediately<SampleMessage>(message);

// Subscribing
messageBusAsReceiver.SubscribeTo<SampleMessage>(Print);
```

#### NonAlloc Variant
```csharp
// Non-allocating message bus setup
var builder = new NonAllocMessageBusBuilder(repositoryFactory, messagePoolBuilder, packedArrayManagedPoolFactory, broadcasterBuilder, nonAllocMessagingFactory);
var messageBus = builder.BuildMessageBus();

// Subscription setup
var subscriptionFactory = new NonAllocSubscriptionFactory(delegateWrapperFactory);
var subscription = subscriptionFactory.BuildSubscriptionSingleArgGeneric<NonAllocSampleMessage>(Print);
messageBusAsReceiver.SubscribeTo<NonAllocSampleMessage>(subscription);

// Sending (similar pattern to standard)
messageBusAsSender.PopMessage<NonAllocSampleMessage>(out var message)
    .Write<NonAllocSampleMessage>(message, messageArgs)
    .SendImmediately<NonAllocSampleMessage>(message);
```

### Threading Models

#### Single-Threaded
- Simple, fast execution
- No synchronization overhead
- Suitable for game main thread

#### Multi-Threaded
- Concurrent message processing
- Thread-safe operations
- Suitable for server scenarios

## Message Types and Patterns

### Command Pattern
```csharp
[Serializable]
public class MoveCommand : IMessage
{
    public Vector3 Direction { get; set; }
    public float Speed { get; set; }
    
    public void Write(params object[] args)
    {
        if (args.Length >= 2)
        {
            Direction = (Vector3)args[0];
            Speed = (float)args[1];
        }
    }
}

// Usage
messageBus.PopMessage<MoveCommand>(out var message)
    .Write<MoveCommand>(message, Vector3.forward, 5.0f)
    .SendImmediately<MoveCommand>(message);
```

### Event Pattern
```csharp
[Serializable]
public class HealthChangedEvent : IMessage
{
    public float OldHealth { get; set; }
    public float NewHealth { get; set; }
    public float MaxHealth { get; set; }
    
    public void Write(params object[] args)
    {
        if (args.Length >= 3)
        {
            OldHealth = (float)args[0];
            NewHealth = (float)args[1];
            MaxHealth = (float)args[2];
        }
    }
}

// Usage
messageBus.PopMessage<HealthChangedEvent>(out var message)
    .Write<HealthChangedEvent>(message, 80f, 60f, 100f)
    .SendImmediately<HealthChangedEvent>(message);
```

### Query Pattern
```csharp
public class GetPlayerStatsQuery
{
    public string PlayerId { get; set; }
    public TaskCompletionSource<PlayerStats> Response { get; set; }
}

// Usage
var query = new GetPlayerStatsQuery 
{ 
    PlayerId = "player1",
    Response = new TaskCompletionSource<PlayerStats>()
};
messageBus.Send(query);
var stats = await query.Response.Task;
```

## Advanced Features

### Message Filtering
```csharp
public class PriorityFilter : IMessageFilter<ICommand>
{
    public bool ShouldProcess(ICommand message)
    {
        return message.Priority >= MinimumPriority;
    }
}
```

### Message Routing
```csharp
public class TopicRouter : IMessageRouter<TopicMessage>
{
    public void Route(TopicMessage message)
    {
        var handlers = GetHandlersForTopic(message.Topic);
        foreach (var handler in handlers)
        {
            handler.Handle(message);
        }
    }
}
```

### Message Transformation
```csharp
public class MessageTransformer<TInput, TOutput>
{
    public TOutput Transform(TInput input)
    {
        // Custom transformation logic
        return transformedMessage;
    }
}
```

## Factory System

### Message Bus Factory Pattern
```csharp
// Standard message bus factory setup
public class MessageBusSetup
{
    public static MessageBus CreateStandardMessageBus(
        RepositoryFactory repositoryFactory,
        ConfigurableStackPoolFactory configurableStackPoolFactory,
        BroadcasterWithRepositoryBuilder broadcasterWithRepositoryBuilder,
        MessagingFactory messagingFactory)
    {
        var builder = new MessageBusBuilder(repositoryFactory, configurableStackPoolFactory, broadcasterWithRepositoryBuilder, messagingFactory);
        builder.New();
        // Add message types as needed
        builder.AddMessageType<YourMessageType>();
        return builder.BuildMessageBus();
    }
    
    public static NonAllocMessageBus CreateNonAllocMessageBus(
        RepositoryFactory repositoryFactory,
        MessagePoolBuilder messagePoolBuilder,
        PackedArrayManagedPoolFactory packedArrayManagedPoolFactory,
        NonAllocBroadcasterBuilder broadcasterBuilder,
        NonAllocMessagingFactory nonAllocMessagingFactory)
    {
        var builder = new NonAllocMessageBusBuilder(repositoryFactory, messagePoolBuilder, packedArrayManagedPoolFactory, broadcasterBuilder, nonAllocMessagingFactory);
        return builder.BuildMessageBus();
    }
}
```

### Publisher/Subscriber (Pinger) Factory
```csharp
// Standard pinger
var pingerFactory = new PingerFactory(repositoryFactory, linkedListBagFactory, broadcasterFactory);
var pinger = pingerFactory.BuildPinger();
var pingerAsPublisher = pinger as IPublisherNoArgs;
var pingerAsSubscribable = pinger as ISubscribableNoArgs;

// Non-allocating pinger
var nonAllocPingerFactory = new NonAllocPingerFactory(repositoryFactory, nonAllocLinkedListBagFactory, nonAllocBroadcasterFactory);
var nonAllocPinger = nonAllocPingerFactory.BuildPinger();
```

## Integration Patterns

### Dependency Injection
```csharp
// Container registration
container.RegisterInstance<IMessageBus>(messageBus);
container.RegisterType<IPublisher<GameEvent>, GameEventPublisher>();

// Constructor injection
public class GameSystem
{
    public GameSystem(IMessageBus messageBus)
    {
        this.messageBus = messageBus;
    }
}
```

### Unity Integration
```csharp
public class MessageBusMonoBehaviour : MonoBehaviour, IMessageBus
{
    private IMessageBus internalBus;
    
    void Awake()
    {
        internalBus = messageBusFactory.CreateStandardBus();
    }
    
    void OnDestroy()
    {
        internalBus.Clear();
    }
}
```

## Best Practices

### Performance Optimization
1. **Use NonAlloc Variants**: For performance-critical paths
2. **Pool Messages**: Reuse message objects when possible
3. **Batch Operations**: Group related messages together
4. **Async Processing**: Use async variants for I/O operations

### Architecture Guidelines
1. **Loose Coupling**: Prefer message passing over direct references
2. **Type Safety**: Use strongly-typed messages
3. **Single Responsibility**: One message type per responsibility
4. **Error Handling**: Implement robust error handling in handlers

### Threading Considerations
1. **Thread Affinity**: Ensure handlers run on appropriate threads
2. **Synchronization**: Use concurrent variants for multi-threaded access
3. **Deadlock Prevention**: Avoid circular message dependencies
4. **Performance Monitoring**: Profile message handling performance

## Error Handling

### Handler Exception Isolation
```csharp
public class SafeMessageHandler<TMessage> : IMessageHandler<TMessage>
{
    public void Handle(TMessage message)
    {
        try
        {
            ProcessMessage(message);
        }
        catch (Exception ex)
        {
            logger.LogError($"Handler error: {ex}");
            // Handler failure doesn't stop other handlers
        }
    }
}
```

### Message Validation
```csharp
public class ValidatingMessageBus : IMessageBus
{
    public void Send<TMessage>(TMessage message)
    {
        if (!ValidateMessage(message))
        {
            throw new InvalidMessageException();
        }
        
        internalBus.Send(message);
    }
}
```

## Related Documentation

- [Delegates and Subscriptions](../Delegates%20and%20subscriptions/DelegatesAndSubscriptions.md) - For event handling patterns
- [Object Pools](../Object%20pools/Object%20pools.md) - For message pooling strategies
- [Asynchronous](../Asynchronous/Asynchronous.md) - For async message patterns
- [Entities](../Entities/Entities.md) - For entity-component messaging integration