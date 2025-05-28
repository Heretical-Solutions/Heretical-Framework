# Time Management Module

## Overview

The Time Management module provides comprehensive time-based operations, timer systems, and synchronization primitives designed for game development scenarios. It includes various timer implementations optimized for different data types and performance requirements.

## Module Structure

### Synchronization.Time (Base)
- **Module Path**: `Synchronization.Time/`
- **Purpose**: Core time management interfaces and utilities
- **Features**: Time providers, time scaling, synchronization primitives
- **Foundation**: Base abstractions for all timing operations

### Timer Implementations

#### Synchronization.Time.Timers
- **Module Path**: `Synchronization.Time.Timers/`
- **Purpose**: Base timer interfaces and common implementations
- **Features**: Generic timer patterns, lifecycle management
- **Abstraction**: Framework-agnostic timer contracts

#### Synchronization.Time.Timers.FloatDelta
- **Module Path**: `Synchronization.Time.Timers.FloatDelta/`
- **Purpose**: Float-based delta time timers optimized for game loops
- **Features**: High-precision floating-point time calculations, frame-rate independence
- **Use Cases**: Smooth animations, physics integration, game mechanics timing
- **Performance**: Optimized for frequent updates with minimal overhead

#### Synchronization.Time.Timers.TimeSpanTick
- **Module Path**: `Synchronization.Time.Timers.TimeSpanTick/`
- **Purpose**: TimeSpan-based tick timers for real-world time precision
- **Features**: .NET TimeSpan precision, system time integration, cross-platform consistency
- **Use Cases**: Real-world time synchronization, scheduling, time-critical operations
- **Accuracy**: System clock precision for time-sensitive applications

#### Synchronization.Time.Timers.TickCollection
- **Module Path**: `Synchronization.Time.Timers.TickCollection/`
- **Purpose**: Collection-based timer management for batch operations
- **Features**: Batch timer operations, efficient timer storage, ordered execution
- **Use Cases**: Managing multiple timers, timer pools, bulk timer operations
- **Scalability**: Handles large numbers of timers efficiently

### Time Management Systems

#### Synchronization.Time.Time updaters
- **Module Path**: `Synchronization.Time.Time updaters/`
- **Purpose**: Time update distribution and centralized time management
- **Features**: Centralized time updates, delta time distribution, time scaling
- **Coordination**: Ensures consistent time updates across all systems

#### Synchronization.Time.Timer managers
- **Module Path**: `Synchronization.Time.Timer managers/`
- **Purpose**: High-level timer lifecycle management and organization
- **Features**: Timer registration, automatic cleanup, pooling integration
- **Management**: Handles timer lifecycles and resource cleanup

### Unity Integration

#### Synchronization.Unity
- **Module Path**: `Synchronization.Unity/`
- **Purpose**: Unity Engine-specific time integration and lifecycle management
- **Features**: MonoBehaviour lifecycle, Unity time providers, coroutine integration
- **Integration**: Seamless Unity Editor and runtime time management

## Core Interfaces

### Time Provider
```csharp
public interface ITimeProvider
{
    float CurrentTime { get; }
    float DeltaTime { get; }
    float TimeScale { get; set; }
    bool IsPaused { get; set; }
}
```

### Float Timer Interface
```csharp
public interface IFloatTimer : ISynchronizableWithDelta<float>
{
    bool Accumulate { get; set; }
    float Duration { get; set; }
    float Elapsed { get; }
    bool IsActive { get; }
    bool IsExpired { get; }
    void Start();
    void Stop();
    void Reset();
}
```

### TimeSpan Timer Interface
```csharp
public interface ITimeSpanTimer : ISynchronizable
{
    bool Accumulate { get; set; }
    TimeSpan Duration { get; set; }
    TimeSpan Elapsed { get; }
    bool IsActive { get; }
    bool IsExpired { get; }
    void Start();
    void Stop();
    void Reset();
}
```

### Timer Manager Interface
```csharp
public interface ITimerManager
{
    string Name { get; }
    void RegisterTimer(ISynchronizable timer);
    void UnregisterTimer(ISynchronizable timer);
    void Clear();
}

public interface ITimeUpdater
{
    string Name { get; }
    bool Togglable { get; }
    bool Active { get; set; }
    bool Scalable { get; }
    bool HasFixedDelta { get; }
    bool CanHaveNegativeDelta { get; }
}
```

## Timer Types

### Float Delta Timers

#### Basic Float Timer
```csharp
public class FloatDeltaTimer : ITimer<float>
{
    public float Duration { get; set; }
    public float Elapsed { get; private set; }
    public float Remaining => Math.Max(0, Duration - Elapsed);
    public bool IsExpired => Elapsed >= Duration;
}
```

#### Features
- **High Performance**: Optimized for game loops
- **Frame-Rate Independence**: Delta time-based calculations
- **Smooth Interpolation**: Ideal for animations and transitions
- **Memory Efficient**: Minimal memory footprint

#### Use Cases
- Animation timers
- Cooldown mechanics
- Smooth transitions
- Frame-rate independent delays

### TimeSpan Tick Timers

#### TimeSpan Timer
```csharp
public class TimeSpanTickTimer : ITimer<TimeSpan>
{
    public TimeSpan Duration { get; set; }
    public TimeSpan Elapsed { get; private set; }
    public TimeSpan Remaining => Duration - Elapsed;
    public bool IsExpired => Elapsed >= Duration;
}
```

#### Features
- **High Precision**: .NET TimeSpan precision
- **Real-World Time**: System clock integration
- **Cross-Platform**: Consistent across platforms
- **DateTime Integration**: Works with .NET date/time APIs

#### Use Cases
- Real-world scheduling
- Time-critical operations
- Server synchronization
- Precise timing requirements

### Tick Collection Timers

#### Collection-Based Management
```csharp
public class TickCollectionTimer
{
    public ICollection<ITick> Ticks { get; }
    public long CurrentTick { get; private set; }
    public void AddTick(ITick tick);
    public void ProcessTicks();
}
```

#### Features
- **Batch Operations**: Process multiple timers efficiently
- **Ordered Execution**: Tick-based ordering
- **Memory Pooling**: Efficient tick reuse
- **Scalable**: Handles large numbers of timers

#### Use Cases
- MMO server timing
- Event scheduling systems
- Batch timer processing
- Performance-critical scenarios

## Timer Lifecycle Management

### Timer Factory Setup and Usage
```csharp
// Factory dependencies
var repositoryFactory = new RepositoryFactory();
var timeUpdaterFactory = new TimeUpdaterFactory(repositoryFactory);
var timerManagerFactory = new TimerManagerFactory(repositoryFactory);
var floatTimerFactory = new FloatTimerFactory();
var timeSpanTimerFactory = new TimeSpanTimerFactory();

// Create time updater
var timeUpdater = timeUpdaterFactory.BuildTimeUpdater(
    "Update", 
    togglable: false, 
    active: true, 
    scalable: false, 
    hasFixedDelta: false, 
    canHaveNegativeDelta: false);

// Create timer manager
var timerManager = timerManagerFactory.BuildTimerManager("Sample timer manager", timeUpdater);

// Create and configure timers
var floatTimer = floatTimerFactory.BuildFloatTimer();
floatTimer.Duration = 5.0f;
floatTimer.Accumulate = true;
floatTimer.Start();

var timeSpanTimer = timeSpanTimerFactory.BuildTimeSpanTimer();
timeSpanTimer.Duration = TimeSpan.FromSeconds(10);
timeSpanTimer.Accumulate = true;
timeSpanTimer.Start();

// Register with manager
timerManager.RegisterTimer(floatTimer);
timerManager.RegisterTimer(timeSpanTimer);
```

### Timer Update Patterns
```csharp
// Float timer direct synchronization
var runtimeTimerSynchronizable = runtimeTimer as ISynchronizableWithDelta<float>;
runtimeTimerSynchronizable.Synchronize(Time.deltaTime);

// TimeSpan timer synchronization
var timerSynchronizable = persistentTimer as ISynchronizable;
timerSynchronizable.Synchronize();

// Manager-based update (handles all registered timers)
// Note: Timer managers in this framework don't expose direct update methods
// They manage timer lifecycle, while synchronization is handled individually

// Unity integration example
public class TimerUpdater : MonoBehaviour
{
    private IFloatTimer gameTimer;
    private ISynchronizableWithDelta<float> gameTimerSynchronizable;
    
    void Start()
    {
        gameTimer = floatTimerFactory.BuildFloatTimer();
        gameTimerSynchronizable = gameTimer as ISynchronizableWithDelta<float>;
        gameTimer.Start();
    }
    
    void Update()
    {
        gameTimerSynchronizable.Synchronize(Time.deltaTime);
        
        if (gameTimer.IsExpired)
        {
            // Handle timer expiration
            gameTimer.Reset();
        }
    }
}
```

### Event Handling
```csharp
public interface ITimerEvents
{
    event Action OnStarted;
    event Action OnStopped;
    event Action OnExpired;
    event Action OnReset;
    event Action<float> OnProgress; // 0.0 to 1.0
}
```

## Advanced Features

### Timer States
```csharp
public enum ETimerState
{
    Idle,
    Running,
    Paused,
    Expired,
    Stopped
}
```

### Timer Metadata
```csharp
public interface ITimerWithMetadata : ITimer
{
    IMetadata Metadata { get; }
    string Name { get; set; }
    int Priority { get; set; }
    object UserData { get; set; }
}
```

### Pooled Timers
```csharp
public interface IPooledTimer : ITimer, IPoolElement
{
    void Reset();
    void Configure(float duration, Action onExpired);
}

// Usage with object pools
using var timer = timerPool.Pop();
timer.Configure(5.0f, () => OnCooldownComplete());
timer.Start();
```

### Chained Timers
```csharp
public class TimerChain
{
    public void AddTimer(ITimer timer, Action onComplete);
    public void Start();
    public void Clear();
}

// Usage
var chain = new TimerChain();
chain.AddTimer(timer1, () => Debug.Log("Step 1"));
chain.AddTimer(timer2, () => Debug.Log("Step 2"));
chain.Start();
```

## Persistence Integration

### Timer Serialization
```csharp
[Serializable]
public class FloatTimerDTO
{
    public float Duration { get; set; }
    public float Elapsed { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
}
```

### Persistent Timer Integration
```csharp
// Timer persistence with settings files
[Serializable]
public class TimerSettings
{
    public float Duration = 10.0f;
    public bool StartImmediately = true;
    public bool AccumulateTime = true;
}

// Integration with persistence system
public class PersistentTimerManager
{
    private IPersistenceService persistenceService;
    private IFloatTimer timer;
    
    public void SaveTimer(string filename)
    {
        var timerData = new FloatTimerDTO
        {
            Duration = timer.Duration,
            Elapsed = timer.Elapsed,
            IsActive = timer.IsActive,
            LastSaveTime = DateTime.Now
        };
        
        persistenceService.Save(timerData, filename);
    }
    
    public void LoadTimer(string filename)
    {
        var timerData = persistenceService.Load<FloatTimerDTO>(filename);
        timer.Duration = timerData.Duration;
        
        // Calculate offline progress
        var offlineTime = DateTime.Now - timerData.LastSaveTime;
        if (timerData.IsActive)
        {
            var totalElapsed = timerData.Elapsed + (float)offlineTime.TotalSeconds;
            // Apply offline progress logic
        }
    }
}

[Serializable]
public class FloatTimerDTO
{
    public float Duration;
    public float Elapsed;
    public bool IsActive;
    public DateTime LastSaveTime;
}
```

## Performance Considerations

### Timer Type Selection

**Use Float Delta Timers for**:
- Game animations and transitions
- Frame-rate independent operations  
- High-frequency updates
- Memory-constrained scenarios

**Use TimeSpan Timers for**:
- Real-world time synchronization
- Precise timing requirements
- Server-side operations
- Cross-platform consistency

**Use Tick Collection Timers for**:
- Large numbers of timers
- Batch processing requirements
- Memory pooling scenarios
- Performance-critical systems

### Optimization Strategies
1. **Pool Timer Objects**: Reuse timer instances
2. **Batch Updates**: Update multiple timers together
3. **Lazy Evaluation**: Only update active timers
4. **Memory Layout**: Use value types where possible
5. **Event Optimization**: Minimize event allocations

## Integration Patterns

### Unity Integration
```csharp
public class UnityTimerProvider : MonoBehaviour, ITimeProvider
{
    public float CurrentTime => Time.time;
    public float DeltaTime => Time.deltaTime;
    public float TimeScale 
    { 
        get => Time.timeScale; 
        set => Time.timeScale = value; 
    }
    public bool IsPaused { get; set; }
}
```

### Dependency Injection
```csharp
// Container registration
container.RegisterType<ITimeProvider, UnityTimeProvider>();
container.RegisterType<ITimerManager<float>, FloatTimerManager>();

// Constructor injection
public class GameSystem
{
    public GameSystem(ITimerManager<float> timerManager)
    {
        this.timerManager = timerManager;
    }
}
```

### ECS Integration
```csharp
// Timer component
public struct TimerComponent
{
    public float Duration;
    public float Elapsed;
    public bool IsActive;
}

// Timer system
public class TimerUpdateSystem : ISystem<float>
{
    public void Update(float deltaTime, World world)
    {
        foreach (var entity in world.GetEntities().With<TimerComponent>())
        {
            ref var timer = ref entity.Get<TimerComponent>();
            if (timer.IsActive)
            {
                timer.Elapsed += deltaTime;
                if (timer.Elapsed >= timer.Duration)
                {
                    // Timer expired
                    HandleTimerExpired(entity);
                }
            }
        }
    }
}
```

## Factory System

### Timer Factory
```csharp
public interface ITimerFactory
{
    ITimer<float> CreateFloatDeltaTimer(float duration);
    ITimer<TimeSpan> CreateTimeSpanTimer(TimeSpan duration);
    ITickCollectionTimer CreateTickCollectionTimer();
}
```

### Builder Pattern
```csharp
public class TimerBuilder<TTimeType>
{
    public TimerBuilder<TTimeType> WithDuration(TTimeType duration);
    public TimerBuilder<TTimeType> WithAutoStart();
    public TimerBuilder<TTimeType> WithPooling();
    public TimerBuilder<TTimeType> WithPersistence();
    public ITimer<TTimeType> Build();
}
```

## Best Practices

### Performance Guidelines
1. **Choose Appropriate Timer Type**: Match timer type to use case
2. **Use Object Pooling**: For frequently created/destroyed timers
3. **Batch Updates**: Update multiple timers in single calls
4. **Minimize Events**: Reduce event handler allocations
5. **Profile Timer Usage**: Monitor timer performance impact

### Architecture Guidelines
1. **Centralized Time Management**: Use timer managers for organization
2. **Consistent Time Sources**: Use single time provider per context
3. **Proper Lifecycle Management**: Clean up timers appropriately
4. **Event-Driven Design**: Use timer events for loose coupling
5. **Persistence Support**: Consider save/load requirements

### Error Handling
1. **Validate Timer State**: Check timer state before operations
2. **Handle Time Overflow**: Manage extreme time values
3. **Exception Safety**: Ensure timer state consistency
4. **Graceful Degradation**: Handle timer failures appropriately

## Related Documentation

- [Object Pools](../Object%20pools/Object%20pools.md) - For timer pooling strategies
- [Persistence](../Persistence/Persistence.md) - For timer serialization
- [Metadata](../Metadata/Metadata.md) - For timer metadata management
- [Entities](../Entities/Entities.md) - For ECS timer integration
- [Unity Integration](../Unity%20Integration/Unity%20Integration.md) - For Unity-specific features