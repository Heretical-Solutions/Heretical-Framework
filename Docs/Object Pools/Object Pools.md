# Object Pools Module

## Overview

The Object Pools module provides a comprehensive, high-performance object pooling system designed to minimize garbage collection pressure and improve performance in memory-sensitive applications. The module uses sophisticated design patterns including decorators, factories, and builders to provide maximum flexibility and extensibility.

## Module Architecture

### Base Variants

#### Object pools (Base)
- **Module Path**: `Object pools/`
- **Purpose**: Core pooling interfaces and basic implementations
- **Features**: Stack, Queue, LinkedList, and PackedArray pool types
- **Threading**: Single-threaded access patterns
- **Memory Management**: Standard heap allocation

#### Object pools.Async
- **Module Path**: `Object pools.Async/`
- **Purpose**: Task-based asynchronous pool operations
- **Features**: Async Pop/Push operations with ExecutionContext support
- **Threading**: Async/await patterns for non-blocking operations
- **Use Cases**: I/O bound operations, background processing

#### Object pools.Managed
- **Module Path**: `Object pools.Managed/`
- **Purpose**: Advanced lifecycle management with metadata tracking
- **Features**: Pool element facades, automatic cleanup, status tracking
- **Complexity**: Higher-level abstraction with rich metadata support
- **Performance**: Slight overhead for enhanced functionality

#### Object pools.Managed.Async
- **Module Path**: `Object pools.Managed.Async/`
- **Purpose**: Asynchronous managed pools combining lifecycle management with async operations
- **Features**: Task-based operations with full lifecycle management
- **Best of Both**: Combines managed pool benefits with async patterns

### Configuration Variants

#### Object pools.Configurable
- **Module Path**: `Object pools.Configurable/`
- **Purpose**: Runtime-configurable pool parameters and allocation strategies
- **Features**: Dynamic allocation strategy changes, resizable pools, runtime growth
- **Flexibility**: Pool behavior can be modified after creation
- **Use Cases**: Adaptive systems, varying load scenarios

#### Object pools.Configurable.Async
- **Module Path**: `Object pools.Configurable.Async/`
- **Purpose**: Asynchronous configurable pools for non-blocking configuration changes
- **Features**: Runtime configuration with async operations, non-blocking resizing
- **Advanced Use Cases**: Server-side pool management, load balancing

### Decorator Variants

#### Object pools.Decorators
- **Module Path**: `Object pools.Decorators/`
- **Purpose**: Base decorator pattern implementations for extensible pool behavior
- **Features**: Composable pool functionality through decorator chaining
- **Pattern**: Decorator pattern allows mixing and matching pool behaviors

#### Object pools.Decorators.Cleanup
- **Module Path**: `Object pools.Decorators.Cleanup/`
- **Purpose**: Automatic object cleanup and state reset on pool operations
- **Features**: `ICleanuppable` integration, configurable cleanup timing
- **Use Cases**: Object state management, preventing data leakage

#### Object pools.Decorators.Concurrent
- **Module Path**: `Object pools.Decorators.Concurrent/`
- **Purpose**: Thread-safe pool operations for multi-threaded environments
- **Features**: Lock-based synchronization, concurrent Pop/Push operations
- **Use Cases**: Multi-threaded game engines, job systems, server applications

#### Object pools.Decorators.Timers
- **Module Path**: `Object pools.Decorators.Timers/`
- **Purpose**: Time-based automatic pool return and timeout management
- **Features**: Configurable timeout durations, automatic Push() on expiration
- **Integration**: Works with framework's timer system for precise timing

#### Object pools.Decorators.Appendable
- **Module Path**: `Object pools.Decorators.Appendable/`
- **Purpose**: Dynamic pool expansion and growth capabilities
- **Features**: Runtime pool size increases, allocation command integration
- **Adaptive Behavior**: Pool grows based on demand patterns

#### Available Decorators
- **Address**: UUID-based pool routing and management for distributed systems
- **Unity**: GameObject-specific pooling with Transform manipulation
- **Variants**: Multi-variant object support within single pool instance

## Core Interfaces

### Basic Pool Interface
```csharp
public interface IPool<T>
{
    T Pop();
    void Push(T item);
    bool TryPop(out T item);
    bool TryPush(T item);
}
```

**Purpose**: Defines the fundamental contract for object pooling operations

**Methods**:
- **`T Pop()`**
  - **Purpose**: Retrieves an object from the pool for use
  - **Returns**: An object instance of type T
  - **Behavior**: Removes object from pool's available instances
  - **Exception**: May throw if pool is empty (implementation-dependent)
  - **Thread Safety**: Depends on pool implementation variant

- **`void Push(T item)`**
  - **Purpose**: Returns an object to the pool for reuse
  - **Parameters**: `item` - The object to return to the pool
  - **Behavior**: Adds object back to pool's available instances
  - **Validation**: Implementation may validate object state
  - **Cleanup**: Object may be reset/cleaned based on pool configuration

- **`bool TryPop(out T item)`**
  - **Purpose**: Safely attempts to retrieve an object without throwing exceptions
  - **Parameters**: `item` - Output parameter receiving the popped object (or default)
  - **Returns**: `true` if object was successfully retrieved, `false` if pool empty
  - **Safety**: Preferred over Pop() for uncertain pool states
  - **Performance**: Slightly faster than exception-based Pop() when pool is empty

- **`bool TryPush(T item)`**
  - **Purpose**: Safely attempts to return an object without throwing exceptions
  - **Parameters**: `item` - The object to return to the pool
  - **Returns**: `true` if object was successfully returned, `false` if pool full/rejected
  - **Use Cases**: Fixed-size pools, validation scenarios

### Resizable Pool Interface
```csharp
public interface IResizable
{
    void Resize(int newSize);
    bool TryResize(int newSize);
}

public interface INumericalResizable : IResizable
{
    int Count { get; }
    int Capacity { get; }
}
```

### Managed Pool Interface
```csharp
public interface IManagedPool<T>
{
    IPoolElementFacade<T> Pop(params object[] args);
    void Push(T item);
    IPoolElementFacade<T> PopFacade();
    void PushFacade(IPoolElementFacade<T> facade);
}
```

**Purpose**: Provides advanced pool management with facades and argument-based object configuration

**Methods**:
- **`IPoolElementFacade<T> Pop(params object[] args)`**
  - **Purpose**: Retrieves a wrapped object with configuration arguments
  - **Parameters**: `args` - Variable arguments for object configuration (passed to ArgumentBuilder)
  - **Returns**: Facade wrapper containing the configured object
  - **Configuration**: Arguments are used to configure object state during retrieval
  - **Lifecycle**: Facade manages object lifecycle and automatic return

- **`void Push(T item)`**
  - **Purpose**: Returns an unwrapped object directly to the pool
  - **Parameters**: `item` - The raw object to return
  - **Use Cases**: When working with objects outside of facade pattern
  - **Cleanup**: Object may undergo cleanup based on pool decorators

- **`IPoolElementFacade<T> PopFacade()`**
  - **Purpose**: Retrieves a facade-wrapped object without configuration arguments
  - **Returns**: Facade wrapper with default object configuration
  - **Usage**: When no special configuration is needed

- **`void PushFacade(IPoolElementFacade<T> facade)`**
  - **Purpose**: Returns a facade-wrapped object to the pool
  - **Parameters**: `facade` - The facade wrapper containing the object to return
  - **Automatic**: Usually called automatically when facade is disposed
  - **Manual Use**: For explicit control over object lifecycle

### Async Pool Interface
```csharp
public interface IAsyncPool<T>
{
    Task<T> PopAsync(AsyncExecutionContext context);
    Task PushAsync(T item, AsyncExecutionContext context);
}
```

## Pool Types

### Stack Pool
- **Implementation**: LIFO (Last In, First Out)
- **Performance**: O(1) for Pop/Push operations
- **Use Cases**: Simple object reuse, temporary allocations
- **Memory**: Compact stack-based storage

### Queue Pool
- **Implementation**: FIFO (First In, First Out)
- **Performance**: O(1) for Pop/Push operations
- **Use Cases**: Fair object distribution, ordered processing
- **Memory**: Circular buffer implementation

### LinkedList Pool
- **Implementation**: Doubly-linked list
- **Performance**: O(1) for Pop/Push operations
- **Use Cases**: Dynamic sizing, frequent insertions/removals
- **Memory**: Node-based allocation

### PackedArray Pool
- **Implementation**: Dense array storage
- **Performance**: O(1) for Pop/Push, cache-friendly
- **Use Cases**: High-performance scenarios, data-oriented design
- **Memory**: Contiguous memory layout

## Pool Element Facade System

### Pool Element Facade Interface
```csharp
public interface IPoolElementFacade<T> : IDisposable
{
    T Value { get; }  // Note: Property name is 'Value' in actual implementation
    void Push();
}
```

**Purpose**: Wraps pooled objects to provide automatic lifecycle management and metadata support

**Properties**:
- **`T Value { get; }`**
  - **Purpose**: Provides access to the actual pooled object
  - **Usage**: Access the wrapped object for normal operations
  - **Lifecycle**: Object remains valid until facade is disposed or pushed
  - **Note**: Property is named 'Value' in the actual implementation

**Methods**:
- **`void Push()`**
  - **Purpose**: Manually returns the wrapped object to the pool
  - **Lifecycle**: Invalidates the facade and returns object for reuse
  - **Cleanup**: Triggers any configured cleanup decorators
  - **Automatic**: Also called automatically during Dispose()

- **`void Dispose()` (from IDisposable)**
  - **Purpose**: Automatic cleanup and object return via using statements
  - **Pattern**: Enables using() patterns for automatic resource management
  - **Implementation**: Typically calls Push() internally
  - **Best Practice**: Prefer using statements over manual Push() calls

### Metadata-Enabled Facade
```csharp
public interface IPoolElementFacadeWithMetadata<T> : IPoolElementFacade<T>
{
    EPoolElementStatus Status { get; }
    IMetadata Metadata { get; }
}
```

### Status Tracking
```csharp
public enum EPoolElementStatus
{
    Uninitialized,
    Popped,
    Pushed
}
```

## Decorator Pattern Implementation

### Base Decorator
```csharp
public abstract class ADecoratorPool<T> : IPool<T>
{
    protected virtual void OnBeforePop() { }
    protected virtual void OnAfterPop(T item) { }
    protected virtual void OnBeforePush(T item) { }
    protected virtual void OnAfterPush() { }
}
```

### Cleanup Decorator
- Automatically calls `ICleanuppable.Cleanup()` on pool operations
- Ensures objects are reset to clean state
- Supports both Pop and Push cleanup strategies

### Concurrent Decorator
- Provides thread-safe access to underlying pools
- Uses appropriate locking mechanisms
- Maintains performance while ensuring safety

### Timer Decorator
- Integrates with the framework's timer system
- Automatically returns objects to pool after timeout
- Configurable timeout durations
- Metadata-based timer tracking

## Factory System

### Basic Factory Pattern
```csharp
public interface IStackPoolFactory<T>
{
    IPool<T> BuildStackPool(IAllocationCommand<T> allocationCommand);
}
```

### Builder Pattern Integration
```csharp
public class PoolBuilder<T>
{
    public PoolBuilder<T> WithStackImplementation();
    public PoolBuilder<T> WithInitialAllocation(IAllocationDescriptor descriptor);
    public PoolBuilder<T> WithCleanupDecorator();
    public PoolBuilder<T> WithConcurrentAccess();
    public IPool<T> Build();
}
```

## Allocation Integration

### Allocation Commands
```csharp
public interface IAllocationCommand<T>
{
    T AllocateObject();
    void ConfigureObject(T obj);
}
```

### Allocation Callbacks
- Pre-allocation hooks for setup
- Post-allocation hooks for configuration
- Integration with dependency injection
- Support for factory-based allocation

## Usage Patterns

### Basic Managed Pool Setup
```csharp
// Pool builder setup with dependency injection
var rootPoolBuilder = new ManagedPoolBuilder<GameObject>(
    allocationCallbackFactory,
    linkedListManagedPoolFactory,
    unityDecoratorPoolFactory,
    managedObjectPoolAllocationCallbackFactory);

// Create pool with decorators and settings
var pool = rootPoolBuilder
    .New()
    .LinkedListManagedPool(linkedListManagedPoolFactory)
    .WithInitial(settings.Initial)
    .WithAdditional(settings.Additional)
    .WithAllocationDelegate(() => UnityZenjectAllocationFactory.DIResolveOrInstantiateAllocationDelegate(container, prefab))
    .DecoratedWithUnityPool(unityDecoratorPoolFactory, null)
    .Build();

// Prepare arguments
var argumentsCache = argumentBuilder.Add(out WorldPositionArgument worldPositionArgument);
worldPositionArgument.WorldPosition = transform.position;

// Use object
var element = pool.Pop(argumentsCache);
// ... use element.Value ...
element.Push(); // or pool.Push(element.Value)
```

### Pool with Multiple Decorators
```csharp
// Complex pool with address, variant, and timer decorators
var pool = rootPoolBuilder
    .New()
    .LinkedListManagedPool(linkedListManagedPoolFactory)
    .WithInitial(settings.Initial)
    .WithAdditional(settings.Additional)
    .WithAllocationDelegate(allocationDelegate)
    .DecoratedWithAddresses<string, GameObject, PoolSettings, ElementSettings>(
        addressDecoratorPoolFactory,
        repositoryFactory,
        stringRepositoryFactory,
        elementSettings)
    .DecoratedWithVariants<GameObject, PoolSettings, VariantSettings>(
        variantDecoratorPoolFactory,
        repositoryFactory,
        poolVariantRepositoryFactory,
        variantSettings)
    .DecoratedWithTimers(
        timerDecoratorPoolFactory,
        floatTimerFactory,
        timerManagerFactory.BuildTimerManager("Pool timer manager", timeUpdater))
    .Build();
```

### Pool Argument System
```csharp
// Setup argument builder
var argumentBuilder = new ArgumentBuilder();

// Add various argument types
var argumentsCache = argumentBuilder
    .Add(out WorldPositionArgument worldPositionArgument)
    .Add(out AddressArgument<string> addressArgument)
    .Add(out VariantArgument<VariantType> variantArgument);

// Configure arguments before use
worldPositionArgument.WorldPosition = Vector3.zero;
addressArgument.Address = "unique_id";
variantArgument.Variant = VariantType.TypeA;

// Use with pool
var element = pool.Pop(argumentsCache);
```

### Pool Settings Configuration
```csharp
[Serializable]
public class PoolSettings : IAllocationAmountSettings
{
    [SerializeField]
    private AllocationCommandDescriptor initial;
    
    [SerializeField]
    private AllocationCommandDescriptor additional;
    
    public AllocationCommandDescriptor Initial => initial;
    public AllocationCommandDescriptor Additional => additional;
}

[Serializable]
public class ElementSettings
{
    // Element-specific configuration
}

[Serializable]
public class VariantSettings
{
    // Variant-specific configuration
}
```

## Performance Considerations

### Pool Type Selection
- **Stack Pool**: Best for simple LIFO scenarios
- **Queue Pool**: Fair distribution, prevents object starvation
- **LinkedList Pool**: Dynamic sizing requirements
- **PackedArray Pool**: Maximum performance, cache-friendly

### Decorator Impact
- **Cleanup**: Minimal overhead for `ICleanuppable` call
- **Concurrent**: Locking overhead, choose appropriate granularity
- **Timer**: Timer registration overhead, metadata storage
- **Multiple Decorators**: Accumulative overhead, profile carefully

### Memory Management
- Pre-allocate pools to avoid runtime allocation
- Choose appropriate initial pool sizes
- Monitor pool hit rates and resize accordingly
- Consider memory vs performance tradeoffs

## Integration with Other Modules

### Collections Module
- Pools use collection implementations for storage
- Benefits from collection performance optimizations
- Shares common interfaces and patterns

### Allocations Module
- Deep integration with allocation command system
- Supports complex allocation strategies
- Factory-based object creation

### Timer Module
- Timer decorator integration
- Automatic object return based on time
- Metadata-based timer tracking

### Metadata Module
- Pool element metadata storage
- Status tracking and lifecycle management
- Cross-cutting concern support

## Integration with Dependency Injection

Pool factories support comprehensive dependency injection through Zenject:

```csharp
// Core factory dependencies
container.Bind<RepositoryFactory>().AsSingle().NonLazy();
container.Bind<AllocationCallbackFactory>().AsSingle().NonLazy();
container.Bind<LinkedListManagedPoolFactory>().AsSingle().NonLazy();
container.Bind<UnityDecoratorPoolFactory>().AsSingle().NonLazy();
container.Bind<ManagedObjectPoolAllocationCallbackFactory>().AsSingle().NonLazy();

// Decorator factory dependencies
container.Bind<AddressDecoratorPoolFactory>().AsSingle().NonLazy();
container.Bind<VariantDecoratorPoolFactory>().AsSingle().NonLazy();
container.Bind<TimerDecoratorPoolFactory>().AsSingle().NonLazy();

// Timer integration
container.Bind<FloatTimerFactory>().AsSingle().NonLazy();
container.Bind<TimerManagerFactory>().AsSingle().NonLazy();
container.Bind<TimeUpdaterFactory>().AsSingle().NonLazy();

// Repository factories for decorators
container.Bind<StringRepositoryFactory>().AsSingle().NonLazy();
container.Bind<PoolVariantRepositoryFactory>().AsSingle().NonLazy();

// Example pool service that uses DI
public class GameObjectPoolService
{
    private readonly ManagedPoolBuilder<GameObject> poolBuilder;
    private readonly Container container;
    
    public GameObjectPoolService(
        ManagedPoolBuilder<GameObject> poolBuilder,
        Container container)
    {
        this.poolBuilder = poolBuilder;
        this.container = container;
    }
    
    public IManagedPool<GameObject> CreatePool(GameObject prefab, PoolSettings settings)
    {
        Func<GameObject> valueAllocationDelegate = () => 
            UnityZenjectAllocationFactory.DIResolveOrInstantiateAllocationDelegate(container, prefab);
            
        return poolBuilder
            .New()
            .LinkedListManagedPool(linkedListManagedPoolFactory)
            .WithInitial(settings.Initial)
            .WithAdditional(settings.Additional)
            .WithAllocationDelegate(valueAllocationDelegate)
            .DecoratedWithUnityPool(unityDecoratorPoolFactory, null)
            .Build();
    }
}
```

## Best Practices

1. **Choose Appropriate Pool Type**: Match pool implementation to usage pattern
2. **Pre-allocate When Possible**: Avoid runtime allocation pressure
3. **Use Managed Pools for Complex Objects**: Leverage automatic cleanup
4. **Profile Decorator Overhead**: Measure actual performance impact
5. **Consider Thread Safety Requirements**: Use concurrent decorators only when needed
6. **Monitor Pool Hit Rates**: Adjust pool sizes based on actual usage
7. **Implement ICleanuppable**: Ensure proper object reset in cleanup decorator

## Related Documentation

- [Allocations](../Allocations/Allocations.md) - For allocation strategy patterns
- [Collections](../Collections/Collections.md) - For underlying storage implementations
- [Metadata](../Metadata/Metadata.md) - For pool element metadata management
- [Time Management](../Time/Time.md) - For timer decorator integration