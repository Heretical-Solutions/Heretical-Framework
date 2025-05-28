# Collections Module

## Overview

The Collections module provides high-performance data structures designed for game development scenarios. It offers multiple variants optimized for different performance characteristics and threading requirements.

## Module Variants

### Collections (Base)
- **Module Path**: `Collections/`
- **Purpose**: Standard collections with full feature sets and safe memory management
- **Use Cases**: General-purpose data storage, prototyping, development environments
- **Memory**: Managed heap allocation with garbage collection
- **Threading**: Single-threaded access patterns
- **Performance**: Good general-purpose performance with GC overhead

### Collections.Concurrent
- **Module Path**: `Collections.Concurrent/`
- **Purpose**: Thread-safe collections for multi-threaded environments
- **Use Cases**: Multi-threaded game engines, job systems, server applications, shared data access
- **Memory**: Thread-safe managed allocation with concurrent optimization
- **Threading**: Lock-free or fine-grained locking mechanisms
- **Performance**: High throughput with synchronization overhead

### Collections.NonAlloc
- **Module Path**: `Collections.NonAlloc/`
- **Purpose**: Zero-allocation collections for performance-critical scenarios
- **Use Cases**: Game loops, mobile optimization, garbage collection sensitive code
- **Memory**: Pre-allocated pools, no runtime allocation after initialization
- **Threading**: Single-threaded optimization for maximum performance
- **Limitations**: Fixed capacity, requires careful sizing

### Collections.NonAlloc.Concurrent
- **Module Path**: `Collections.NonAlloc.Concurrent/`
- **Purpose**: Thread-safe, zero-allocation collections
- **Use Cases**: High-performance multi-threaded scenarios, concurrent game engine systems
- **Memory**: Pre-allocated thread-safe pools
- **Threading**: Lock-free concurrent access with zero allocation
- **Complexity**: Most complex variant, requires careful capacity planning

### Collections.Unmanaged
- **Module Path**: `Collections.Unmanaged/`
- **Purpose**: Native memory collections for maximum performance and control
- **Use Cases**: Data-oriented design, large datasets, memory-constrained environments
- **Memory**: Manual native memory management, no GC pressure
- **Threading**: Implementation-dependent, manual synchronization
- **Responsibility**: Developer must handle memory lifecycle explicitly

## Core Data Structures

### B+ Trees
- **Standard**: `BPlusTree<TKey, TValue>`
- **Concurrent**: `ConcurrentBPlusTree<TKey, TValue>`
- **NonAlloc**: `NonAllocBPlusTree<TKey, TValue>`
- **Unmanaged**: `UnmanagedBPlusTree<TKey, TValue>`

**Core Interface**:
```csharp
public interface IBPlusTree<TKey, TValue>
{
    void Insert(TKey key, TValue value);
    bool TryGetValue(TKey key, out TValue value);
    bool Remove(TKey key);
    bool ContainsKey(TKey key);
    IEnumerable<KeyValuePair<TKey, TValue>> GetRange(TKey startKey, TKey endKey);
    int Count { get; }
    void Clear();
}
```

**Purpose**: Self-balancing tree structure optimized for sorted data with efficient range queries

**Key Methods**:
- **`Insert(TKey key, TValue value)`**: Adds or updates key-value pair while maintaining sort order
- **`TryGetValue(TKey key, out TValue value)`**: Safe retrieval with O(log n) complexity
- **`GetRange(TKey startKey, TKey endKey)`**: Efficient range queries for sorted data
- **`Remove(TKey key)`**: Removes key while maintaining tree balance

**Features**:
- **Sorted Storage**: Maintains keys in sorted order automatically
- **Range Queries**: Efficient iteration over key ranges
- **Cache-Friendly**: Node layout optimized for CPU cache performance
- **Custom Comparers**: Support for custom key comparison logic
- **Self-Balancing**: Maintains optimal tree structure automatically

**Use Cases**:
- **Spatial Indexing**: 2D/3D spatial queries with coordinate keys
- **Database-like Queries**: Range searches, sorted iteration
- **Time-based Data**: Events, logs with timestamp keys
- **Sorted Collections**: When insertion order doesn't matter but sorted access does

### Circular Buffers
- **Standard**: `CircularBuffer<T>`
- **Concurrent**: `ConcurrentCircularBuffer<T>`
- **NonAlloc**: `NonAllocCircularBuffer<T>`
- **Unmanaged**: `UnmanagedCircularBuffer<T>`

**Core Interface**:
```csharp
public interface ICircularBuffer<T>
{
    void Push(T item);
    bool TryPop(out T item);
    bool TryPeek(out T item);
    T this[int index] { get; set; }
    int Count { get; }
    int Capacity { get; }
    bool IsFull { get; }
    bool IsEmpty { get; }
    void Clear();
}
```

**Purpose**: Fixed-size ring buffer with automatic overwrite behavior for continuous data streams

**Key Methods**:
- **`Push(T item)`**: Adds item to buffer, overwrites oldest if full
- **`TryPop(out T item)`**: Removes and returns oldest item (FIFO)
- **`TryPeek(out T item)`**: Views oldest item without removing
- **`this[int index]`**: Direct access to buffer elements by relative index

**Properties**:
- **`Count`**: Current number of items in buffer
- **`Capacity`**: Maximum buffer size (fixed at creation)
- **`IsFull`**: True when buffer has reached maximum capacity
- **`IsEmpty`**: True when buffer contains no items

**Features**:
- **Fixed-Size**: Memory footprint known at compile time
- **Ring Buffer**: Efficient FIFO with automatic wrap-around
- **Overwrite Behavior**: Automatically overwrites oldest data when full
- **Lock-Free**: Concurrent variants use atomic operations
- **Cache-Friendly**: Contiguous memory layout for optimal performance

**Use Cases**:
- **Audio/Video Streaming**: Continuous data buffers with fixed latency
- **Event History**: Rolling window of recent events/inputs
- **Performance Metrics**: Moving averages, frame time tracking
- **Temporal Data**: Time-series data with fixed memory usage
- **Network Buffers**: Packet buffering with overflow protection

## Factory System

Each collection variant provides factory interfaces for dependency injection:

```csharp
// Example factory interfaces
public interface IBPlusTreeFactory<TKey, TValue>
{
    IBPlusTree<TKey, TValue> BuildBPlusTree(/* parameters */);
}

public interface ICircularBufferFactory<T>
{
    ICircularBuffer<T> BuildCircularBuffer(int capacity);
}
```

## Performance Characteristics

### Standard Collections
- **Memory**: Managed heap allocation
- **Threading**: Thread-safe with locking
- **GC Impact**: Standard garbage collection pressure
- **Performance**: Good general-purpose performance

### Concurrent Collections
- **Memory**: Managed heap with concurrent access patterns
- **Threading**: Lock-free or fine-grained locking
- **GC Impact**: Optimized for concurrent access
- **Performance**: High throughput in multi-threaded scenarios

### NonAlloc Collections
- **Memory**: Pre-allocated pools, no runtime allocation
- **Threading**: Single-threaded optimization
- **GC Impact**: Zero garbage generation after initialization
- **Performance**: Optimal for tight game loops

### Unmanaged Collections
- **Memory**: Native memory, manual management
- **Threading**: Depends on implementation
- **GC Impact**: No garbage collection pressure
- **Performance**: Maximum performance for large datasets

## Usage Guidelines

### Choosing the Right Variant

**Use Standard Collections when**:
- Prototyping or development
- Performance is not critical
- Simplicity is preferred

**Use Concurrent Collections when**:
- Multiple threads access the same data
- Multi-threaded game engines (job systems, worker threads)
- Server-side applications
- Background processing scenarios

**Use NonAlloc Collections when**:
- In performance-critical game loops
- Mobile/console optimization is required
- Garbage collection needs to be minimized

**Use Unmanaged Collections when**:
- Working with large datasets
- Maximum performance is required
- Memory layout control is important

### Integration with Other Modules

The Collections module integrates seamlessly with:
- **Object Pools**: Collections can be pooled for reuse
- **Persistence**: Collections support serialization
- **Messaging**: Collections used for message queues
- **Repositories**: Collections as backing stores

### Best Practices

1. **Choose Appropriate Capacity**: Pre-size collections when possible
2. **Use Generic Constraints**: Leverage type constraints for performance
3. **Consider Memory Layout**: Structure data for cache efficiency
4. **Profile Performance**: Measure actual performance impact
5. **Avoid Premature Optimization**: Start with standard variants

## Examples

### Collection Factory Usage
```csharp
// Factory setup with dependencies
var repositoryFactory = new RepositoryFactory();
var bPlusTreeFactory = new BPlusTreeFactory<int, string>(repositoryFactory);
var circularBufferFactory = new CircularBufferFactory<float>();

// Standard collection creation
var tree = bPlusTreeFactory.BuildBPlusTree(/* configuration */);
tree.Insert(1, "value1");
tree.Insert(2, "value2");

// Circular buffer usage
var buffer = circularBufferFactory.BuildCircularBuffer(1024);
buffer.Push(3.14f);
buffer.Push(2.71f);
```

### Concurrent Collection Access
```csharp
// Thread-safe operations for multi-threaded engines
var concurrentFactory = new ConcurrentBPlusTreeFactory<int, GameEvent>(repositoryFactory);
var concurrentTree = concurrentFactory.BuildBPlusTree(/* configuration */);

// Safe concurrent access from multiple threads
Task.Run(() => concurrentTree.Insert(key, event));
Task.Run(() => concurrentTree.TryGetValue(key, out var value));

// Ideal for job systems in game engines
var jobSystem = new JobSystem();
jobSystem.Schedule(() => concurrentTree.ProcessRange(startKey, endKey));
```

### Unmanaged Collections
```csharp
// High-performance unmanaged memory for data-oriented design
var memoryPool = new MemoryPool();
var unmanagedArray = new UnmanagedArray<Vector3>(memoryPool, 4096);
unmanagedArray.Push(new Vector3(1, 2, 3));

// Cleanup when done
unmanagedArray.Dispose();
```

## Integration with Dependency Injection

Collection factories integrate with dependency injection through factory chains:

```csharp
// Zenject container setup for collection dependencies
container.Bind<RepositoryFactory>().AsSingle().NonLazy();
container.Bind<BPlusTreeFactory<int, string>>().AsSingle();
container.Bind<CircularBufferFactory<float>>().AsSingle();
container.Bind<ConfigurableStackPoolFactory>().AsSingle().NonLazy();

// Concurrent variants
container.Bind<ConcurrentBPlusTreeFactory<int, GameEvent>>().AsSingle();
container.Bind<ConcurrentCircularBufferFactory<Vector3>>().AsSingle();

// Example usage in a system
public class GameDataSystem
{
    private readonly BPlusTreeFactory<int, PlayerData> playerDataFactory;
    private readonly CircularBufferFactory<InputEvent> inputBufferFactory;
    
    public GameDataSystem(
        BPlusTreeFactory<int, PlayerData> playerDataFactory,
        CircularBufferFactory<InputEvent> inputBufferFactory)
    {
        this.playerDataFactory = playerDataFactory;
        this.inputBufferFactory = inputBufferFactory;
    }
    
    public void Initialize()
    {
        var playerDatabase = playerDataFactory.BuildBPlusTree(/* config */);
        var inputBuffer = inputBufferFactory.BuildCircularBuffer(1024);
    }
}
```

## Related Documentation

- [Object Pools](../Object%20pools/Object%20pools.md) - For collection reuse patterns
- [Allocations](../Allocations/Allocations.md) - For memory allocation strategies
- [Persistence](../Persistence/Persistence.md) - For collection serialization