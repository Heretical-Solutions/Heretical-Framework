# ECS and Entities Module

## Overview

The ECS and Entities module provides Entity-Component-System architecture support with a focus on modularity and engine independence. While DefaultECS is the primary supported implementation, the framework is designed to accommodate multiple ECS frameworks through modular architecture.

## Module Structure

### Entities (Base)
- **Location**: `Assets/Scripts/Framework/Entities/`
- **Purpose**: Engine-agnostic ECS patterns and interfaces
- **Features**: Entity management abstractions, world management interfaces

### Entities.DefaultECS
- **Location**: `Assets/Scripts/Framework/Entities.DefaultECS/` (implied)
- **Purpose**: DefaultECS-specific implementations
- **Features**: DefaultECS integration, world management, entity factories

### Entities.Networking
- **Location**: `Assets/Scripts/Framework/Entities.Networking/`
- **Purpose**: Network-aware entity systems
- **Features**: Entity synchronization, network entity management, distributed systems

## Core Concepts

### Multi-World Architecture

The framework implements a sophisticated multi-world ECS pattern:

#### Registry World
- **Purpose**: Central entity tracking and ID management
- **Responsibilities**: Cross-world entity relationships, shared entity IDs
- **Features**: Entity registration, world coordination, ID allocation

#### Simulation World
- **Purpose**: Game logic execution and state management
- **Responsibilities**: Game systems, logic processing, state updates
- **Features**: Core gameplay systems, AI, physics simulation

#### View World
- **Purpose**: Visual representation and rendering
- **Responsibilities**: Rendering, animations, visual effects
- **Features**: Render components, visual systems, presentation layer

#### Event World
- **Purpose**: Event storage and cross-world communication
- **Responsibilities**: Event handling, messaging, system coordination
- **Features**: Event components, messaging systems, inter-world communication

### Shared Entity System

#### SharedEntityID
Entities can exist across multiple worlds with shared identifiers:
- **Cross-World References**: Entities reference counterparts in other worlds
- **Synchronized State**: Changes propagate between related entities
- **Lifecycle Management**: Coordinated creation/destruction across worlds

## Core Interfaces

### Entity Management
```csharp
public interface IEntityManager
{
    Entity CreateEntity();
    Entity CreateEntity(EntityDescriptor descriptor);
    void DestroyEntity(Entity entity);
    bool IsEntityAlive(Entity entity);
}
```

### World Management
```csharp
public interface IWorldManager
{
    IWorld GetWorld(string worldName);
    IWorld CreateWorld(string worldName);
    void DestroyWorld(string worldName);
    IEnumerable<IWorld> GetAllWorlds();
}
```

### Entity Prototype System
```csharp
public interface IEntityPrototype
{
    Entity CreateEntity(IWorld world);
    void ConfigureEntity(Entity entity);
    EntityDescriptor GetDescriptor();
}
```

### Entity Factory
```csharp
public interface IEntityFactory
{
    Entity CreateFromPrototype(IEntityPrototype prototype);
    Entity CreateFromDescriptor(EntityDescriptor descriptor);
    void DestroyEntity(Entity entity);
}
```

## Entity Authoring and Prototypes

### Entity Authoring Presets
```csharp
public enum EEntityAuthoringPresets
{
    SimulationOnly,
    ViewOnly,
    SimulationAndView,
    AllWorlds,
    Custom
}
```

### Entity Prototype Management
- **Prototype Storage**: Centralized prototype repository
- **Prototype Inheritance**: Hierarchical prototype relationships
- **Runtime Modification**: Dynamic prototype updates
- **Serialization Support**: Save/load prototype definitions

### Entity Descriptor System
```csharp
public class EntityDescriptor
{
    public string Name { get; set; }
    public List<ComponentDescriptor> Components { get; set; }
    public EEntityAuthoringPresets Preset { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

## World Management System

### World Controllers
```csharp
public interface IWorldController
{
    void Initialize(IWorld world);
    void Update(float deltaTime);
    void Shutdown();
    IWorld World { get; }
}
```

### World Override System
```csharp
public class WorldOverrideDescriptor
{
    public string WorldName { get; set; }
    public Type[] SystemOverrides { get; set; }
    public ComponentOverride[] ComponentOverrides { get; set; }
}
```

### Cross-World Synchronization
- **Entity Mapping**: Maintain entity relationships across worlds
- **Component Synchronization**: Keep related components in sync
- **Event Propagation**: Distribute events between worlds
- **Lifecycle Coordination**: Manage entity creation/destruction

## DefaultECS Integration

### World Setup
```csharp
// Create worlds with DefaultECS
var registryWorld = new DefaultEcsWorld();
var simulationWorld = new DefaultEcsWorld();
var viewWorld = new DefaultEcsWorld();
var eventWorld = new DefaultEcsWorld();

// Register with world manager
worldManager.RegisterWorld("Registry", registryWorld);
worldManager.RegisterWorld("Simulation", simulationWorld);
worldManager.RegisterWorld("View", viewWorld);
worldManager.RegisterWorld("Event", eventWorld);
```

### Entity Creation with Shared IDs
```csharp
// Create entity in multiple worlds
var sharedId = idGenerator.GenerateId();

var simEntity = simulationWorld.CreateEntity()
    .Set(new SharedEntityID { Id = sharedId })
    .Set(new PositionComponent { X = 10, Y = 20 });

var viewEntity = viewWorld.CreateEntity()
    .Set(new SharedEntityID { Id = sharedId })
    .Set(new SpriteComponent { TextureId = "player" });

// Register relationship
entityRegistry.RegisterRelationship(sharedId, simEntity, viewEntity);
```

### System Integration
```csharp
public class MovementSystem : ISystem<float>
{
    private readonly ISet<Entity> entities;
    
    public MovementSystem(World world)
    {
        entities = world.GetEntities()
            .With<PositionComponent>()
            .With<VelocityComponent>()
            .AsSet();
    }
    
    public void Update(float deltaTime)
    {
        foreach (var entity in entities.GetEntities())
        {
            ref var position = ref entity.Get<PositionComponent>();
            ref var velocity = ref entity.Get<VelocityComponent>();
            
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;
        }
    }
}
```

## Networking Integration

### Network Entity Management
```csharp
public interface INetworkEntityManager
{
    void RegisterNetworkEntity(Entity entity, NetworkId networkId);
    void UnregisterNetworkEntity(NetworkId networkId);
    Entity GetEntityByNetworkId(NetworkId networkId);
    void SynchronizeEntity(NetworkId networkId);
}
```

### Entity Synchronization
- **State Synchronization**: Sync component data across network
- **Event Synchronization**: Distribute events over network
- **Ownership Management**: Track entity ownership across clients
- **Conflict Resolution**: Handle concurrent entity modifications

### Network Components
```csharp
[NetworkComponent]
public struct NetworkPositionComponent
{
    public float X, Y, Z;
    public uint Timestamp;
    public bool IsDirty;
}

[NetworkComponent]
public struct NetworkOwnershipComponent
{
    public uint OwnerId;
    public NetworkAuthority Authority;
}
```

## Managed Type Resource System

### Resource Management
```csharp
public interface IManagedTypeResourceManager
{
    TResource GetResource<TResource>() where TResource : class;
    void RegisterResource<TResource>(TResource resource) where TResource : class;
    void UnregisterResource<TResource>() where TResource : class;
}
```

### Resource Types
- **Asset Resources**: Texture, audio, model references
- **Configuration Resources**: Game settings, parameters
- **Service Resources**: Logging, networking, persistence services
- **System Resources**: Shared system instances

## Settings and Configuration

### Entity World Settings
```csharp
public class EntityWorldSettings
{
    public int InitialEntityCapacity { get; set; }
    public bool EnableMultiThreading { get; set; }
    public string[] ActiveSystems { get; set; }
    public WorldSyncSettings SyncSettings { get; set; }
}
```

### World Synchronization Settings
```csharp
public class WorldSyncSettings
{
    public bool AutoSyncComponents { get; set; }
    public float SyncInterval { get; set; }
    public string[] SynchronizedComponentTypes { get; set; }
}
```

## Persistence Integration

### Entity Serialization
```csharp
public interface IEntitySerializer
{
    EntityDTO Serialize(Entity entity);
    Entity Deserialize(EntityDTO dto, IWorld world);
    void SerializeWorld(IWorld world, Stream stream);
    void DeserializeWorld(Stream stream, IWorld world);
}
```

### World State Management
- **Save/Load**: Complete world state persistence
- **Incremental Saves**: Save only changed entities
- **Versioning**: Handle save format changes
- **Compression**: Optimize save file sizes

## Factory System

### Entity Factory Builder
```csharp
public class EntityFactoryBuilder
{
    public EntityFactoryBuilder WithPrototype(IEntityPrototype prototype);
    public EntityFactoryBuilder WithComponentInitializer<T>(Action<T> initializer);
    public EntityFactoryBuilder WithWorld(IWorld world);
    public IEntityFactory Build();
}
```

### Prototype Factory
```csharp
public interface IPrototypeFactory
{
    IEntityPrototype CreatePrototype(string name);
    IEntityPrototype LoadPrototype(string fileName);
    void SavePrototype(IEntityPrototype prototype, string fileName);
}
```

## Performance Considerations

### ECS Framework Selection
**DefaultECS Benefits**:
- High performance
- Mature and stable
- Good Unity integration
- Strong typing support

**Framework Independence Benefits**:
- Flexibility to switch ECS frameworks
- Reduced vendor lock-in
- Framework-specific optimizations
- Learning curve accommodation

### Multi-World Performance
- **Memory Overhead**: Multiple worlds require more memory
- **Synchronization Cost**: Cross-world sync has performance impact
- **System Complexity**: More complex system management
- **Benefits**: Better separation of concerns, easier debugging

### Optimization Strategies
1. **Minimize Cross-World Sync**: Only sync necessary data
2. **Batch Operations**: Group related entity operations
3. **Component Pooling**: Reuse component instances
4. **System Ordering**: Optimize system execution order
5. **Memory Layout**: Consider component memory patterns

## Integration Patterns

### Dependency Injection
```csharp
// Register ECS services
container.RegisterType<IWorldManager, DefaultEcsWorldManager>();
container.RegisterType<IEntityFactory, DefaultEcsEntityFactory>();
container.RegisterInstance<IWorld>(simulationWorld);

// System injection
public class GameplaySystem
{
    public GameplaySystem(IWorldManager worldManager, IEntityFactory entityFactory)
    {
        this.worldManager = worldManager;
        this.entityFactory = entityFactory;
    }
}
```

### Messaging Integration
```csharp
// Entity events through messaging system
public class EntityCreatedEvent
{
    public Entity Entity { get; set; }
    public string WorldName { get; set; }
    public EntityDescriptor Descriptor { get; set; }
}

// System event handling
messageBus.Subscribe<EntityCreatedEvent>(OnEntityCreated);
```

### Unity Integration
```csharp
public class EcsManager : MonoBehaviour
{
    private IWorldManager worldManager;
    private List<ISystem> systems;
    
    void Start()
    {
        InitializeWorlds();
        InitializeSystems();
    }
    
    void Update()
    {
        UpdateSystems(Time.deltaTime);
    }
    
    void OnDestroy()
    {
        CleanupSystems();
        CleanupWorlds();
    }
}
```

## Best Practices

### Architecture Guidelines
1. **Separate Concerns**: Use different worlds for different purposes
2. **Minimize Cross-World Dependencies**: Keep worlds loosely coupled
3. **Use Shared IDs Wisely**: Only when cross-world references needed
4. **Component Design**: Keep components data-only where possible
5. **System Organization**: Group related systems by world

### Performance Guidelines
1. **Profile ECS Usage**: Monitor system and component performance
2. **Optimize Hot Paths**: Focus on frequently executed systems
3. **Component Size**: Keep components small and cache-friendly
4. **System Dependencies**: Minimize system interdependencies
5. **Memory Management**: Use object pooling for entities/components

### Debugging and Development
1. **Entity Inspection**: Implement entity debugging tools
2. **World Visualization**: Visualize world relationships
3. **Performance Monitoring**: Track system execution times
4. **Component Validation**: Validate component data integrity
5. **Event Tracing**: Log entity lifecycle events

## Related Documentation

- [Messaging](../Messaging/Messaging.md) - For entity event communication
- [Object Pools](../Object%20pools/Object%20pools.md) - For entity/component pooling
- [Persistence](../Persistence/Persistence.md) - For entity serialization
- [Networking](../Networking/Networking.md) - For distributed entity systems
- [Unity Integration](../Unity%20Integration/Unity%20Integration.md) - For Unity-specific ECS features