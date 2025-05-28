# Framework Documentation Index

## Overview

This documentation covers the Heretical Unity Projects Framework - a comprehensive, engine-agnostic game development toolkit designed for modularity, performance, and flexibility across different environments (POCO, Unity, and potentially other engines).

## Getting Started

### Essential Reading
1. **[Framework Architecture Overview](./Framework%20Architecture%20Overview.md)** - Start here for high-level understanding
2. **[Collections](./Collections/Collections.md)** - Core data structures used throughout
3. **[Object Pools](./Object%20pools/Object%20pools.md)** - Memory management fundamentals
4. **[Messaging](./Messaging/Messaging.md)** - Communication patterns

## Module Documentation

### Core Infrastructure
| Module | Purpose | Performance Variants | Engine Support |
|--------|---------|---------------------|----------------|
| **[Collections](./Collections/Collections.md)** | High-performance data structures | Standard, Concurrent, NonAlloc, Unmanaged | Engine-agnostic |
| **[Extensions and Helpers](./Extensions%20and%20Helpers/Extensions%20and%20Helpers.md)** | Utility methods and extensions | Base, Unity-specific | POCO + Unity |
| **[Allocations](./Allocations/Allocations.md)** | Memory allocation strategies | Standard, Async, Unity, Zenject | Cross-platform |

### Memory Management
| Module | Purpose | Key Features | Use Cases |
|--------|---------|-------------|-----------|
| **[Object Pools](./Object%20pools/Object%20pools.md)** | Object pooling and reuse | Decorators, Async, Managed, Configurable | GC optimization, performance |
| **[Object Bags](./Object%20Bags/Object%20Bags.md)** | Dynamic object containers | Concurrent, NonAlloc variants | Dynamic collections |
| **[Collections Unmanaged](./Collections%20Unmanaged/Collections%20Unmanaged.md)** | Native memory collections | High-performance, manual management | Data-oriented design |

### Communication Systems
| Module | Purpose | Threading Support | Allocation Strategy |
|--------|---------|------------------|-------------------|
| **[Messaging](./Messaging/Messaging.md)** | Event bus and message passing | Single/Multi-threaded | Allocating, NonAlloc |
| **[Delegates and Subscriptions](./Delegates%20and%20subscriptions/DelegatesAndSubscriptions.md)** | Event handling patterns | Concurrent variants | Multiple allocation strategies |
| **[Blackboard](./Blackboard/Blackboard.md)** | Shared data storage | Unity integration | Engine-specific variants |

### Data Management
| Module | Purpose | Serialization Formats | Repository Types |
|--------|---------|---------------------|------------------|
| **[Databases and Repositories](./Databases%20and%20repositories/DatabasesAndRepositories.md)** | Repository pattern implementations | Multiple backends | Standard, Concurrent |
| **[Persistence](./Persistence/Persistence.md)** | Serialization and data storage | JSON, XML, CSV, YAML, Protobuf, Binary | Visitor pattern |
| **[Metadata](./Metadata/Metadata.md)** | Dynamic metadata management | Strong/weak typing | Cross-cutting concerns |

### State Management
| Module | Purpose | State Types | Integration |
|--------|---------|------------|-------------|
| **[FSM](./State%20machines/StateMachines.md)** | Finite state machines | Regular, Concurrent, NonAlloc, Async | Builder pattern |
| **[Time Management](./Time%20Management/Time%20Management.md)** | Timer systems and synchronization | Float, TimeSpan, Tick collections | Performance-optimized |
| **[Synchronization](./Synchronization/Synchronization.md)** | Coordination primitives | Time updaters, managers | Cross-system coordination |

### Game Development
| Module | Purpose | ECS Support | Networking |
|--------|---------|------------|------------|
| **[ECS and Entities](./ECS%20and%20Entities/ECS%20and%20Entities.md)** | Entity-Component-System | DefaultECS, Modular design | Network entities |
| **[MVVM](./MVVM/MVVM.md)** | Model-View-ViewModel pattern | Unity UI, UIToolkit | Data binding |
| **[Systems and Procedures](./Systems%20and%20Procedures/Systems%20and%20Procedures.md)** | Game logic execution | Async, Delegate variants | System orchestration |

### Specialized Systems
| Module | Purpose | Platform Support | Integration |
|--------|---------|-----------------|-------------|
| **[Asset Import](./Asset%20Import/Asset%20Import.md)** | Asset pipeline management | Unity-specific | Concurrent processing |
| **[Lifetime Management](./Lifetime%20management/LifetimeManagement.md)** | Object lifecycle | POCO, Unity, Zenject | DI integration |
| **[IK](./IK/IK.md)** | Inverse kinematics | Unity integration | Animation systems |
| **[Networking](./Networking/Networking.md)** | Network communication | LiteNetLib integration | ECS networking |

## Architecture Patterns

### Performance Variants Strategy
Most modules follow a consistent pattern of providing multiple implementation variants:

- **Standard**: Feature-complete, general-purpose implementation
- **Concurrent**: Thread-safe variants for multi-threaded scenarios
- **NonAlloc**: Zero-allocation variants for performance-critical paths
- **Async**: Asynchronous variants for long-running operations
- **Unity**: Unity Engine-specific optimizations and integrations

### Design Patterns Used

#### Creational Patterns
- **Factory Pattern**: Object creation with dependency resolution
- **Builder Pattern**: Fluent configuration APIs
- **Pool Pattern**: Object reuse for performance optimization

#### Structural Patterns
- **Decorator Pattern**: Extensible functionality addition
- **Facade Pattern**: Simplified interfaces for complex subsystems
- **Repository Pattern**: Data access abstraction

#### Behavioral Patterns
- **Command Pattern**: Encapsulated operations and undo functionality
- **Observer Pattern**: Event-driven communication
- **State Machine Pattern**: State management and transitions
- **Visitor Pattern**: Operations on data structures

### Cross-Module Integration

#### Dependency Injection
- **Autofac**: Primary DI container for POCO environments
- **Zenject**: Unity-specific dependency injection
- **Interface-Based**: All major systems expose interfaces

#### Event-Driven Architecture
- Messaging system provides backbone for inter-system communication
- Delegates and subscriptions for local event handling
- Blackboard system for shared state management

#### Performance Optimization
- Object pooling integrated throughout framework
- NonAlloc variants minimize garbage collection
- Data-oriented design principles applied consistently

## Development Guides

### Getting Started Paths

#### For Unity Developers
1. Start with **[Framework Architecture Overview](./Framework%20Architecture%20Overview.md)**
2. Learn **[Unity Integration Patterns](./Unity%20Integration/Unity%20Integration.md)**
3. Study **[Object Pools](./Object%20pools/Object%20pools.md)** for performance
4. Implement **[Messaging](./Messaging/Messaging.md)** for communication

#### For POCO Developers
1. Read **[Framework Architecture Overview](./Framework%20Architecture%20Overview.md)**
2. Understand **[Collections](./Collections/Collections.md)** fundamentals
3. Learn **[Dependency Injection](./DI/Dependency%20Injection.md)** patterns
4. Explore **[ECS and Entities](./ECS%20and%20Entities/ECS%20and%20Entities.md)** if using ECS

#### For Performance-Critical Applications
1. Focus on **[NonAlloc variants](./Performance/NonAlloc%20Patterns.md)**
2. Study **[Object Pools](./Object%20pools/Object%20pools.md)** extensively
3. Learn **[Unmanaged Collections](./Collections%20Unmanaged/Collections%20Unmanaged.md)**
4. Apply **[Data-Oriented Design](./Performance/Data%20Oriented%20Design.md)** principles

### Common Integration Scenarios

#### Game Systems Communication
```
Entity System → Messaging → UI System
     ↓              ↓           ↓
 Game State → Blackboard → Audio System
```

#### Memory Management Flow
```
Allocation Commands → Object Pools → Collections → Cleanup
        ↓                  ↓           ↓         ↓
   Configuration → Pool Decorators → Timers → GC Optimization
```

#### State Management Pipeline
```
Input → FSM → Entity Components → Persistence
  ↓      ↓           ↓              ↓
Events → Timers → Messaging → Save System
```

## Best Practices Summary

### Performance Guidelines
1. **Choose Appropriate Variants**: Match performance characteristics to requirements
2. **Use Object Pooling**: For frequently created/destroyed objects
3. **Minimize Allocations**: Prefer NonAlloc variants in hot paths
4. **Profile Regularly**: Measure actual performance impact
5. **Consider Threading**: Use concurrent variants only when needed

### Architecture Guidelines
1. **Program to Interfaces**: Use interfaces for maximum flexibility
2. **Favor Composition**: Use composition over inheritance
3. **Loose Coupling**: Prefer messaging over direct references
4. **Single Responsibility**: Keep modules focused on specific concerns
5. **Data-Oriented Design**: Structure data for cache efficiency

### Integration Guidelines
1. **Start Simple**: Begin with standard variants, optimize later
2. **Modular Approach**: Add only needed modules to reduce complexity
3. **Consistent Patterns**: Follow established framework patterns
4. **Error Handling**: Implement robust error handling throughout
5. **Documentation**: Document architectural decisions and patterns

## Troubleshooting

### Common Issues
- **Performance Problems**: Check variant selection, profiling guides
- **Memory Leaks**: Review object pool usage, disposal patterns
- **Threading Issues**: Verify concurrent variant usage
- **Integration Problems**: Check dependency injection configuration

### Debugging Resources
- **[Profiling Guide](./Debugging/Profiling%20Guide.md)** - Performance analysis
- **[Common Patterns](./Debugging/Common%20Patterns.md)** - Typical usage patterns
- **[Error Handling](./Debugging/Error%20Handling.md)** - Exception management

## Version Information

- **Framework Version**: Latest
- **Unity Compatibility**: 2021.3.25f1 LTS and newer
- **.NET Compatibility**: .NET Standard 2.1, .NET Framework 4.8
- **ECS Support**: DefaultECS 0.17.2 (with modular support for other frameworks)

## Contributing

For contributing guidelines and development setup, see:
- **[Development Setup](./Contributing/Development%20Setup.md)**
- **[Coding Standards](./Contributing/Coding%20Standards.md)**
- **[Testing Guidelines](./Contributing/Testing%20Guidelines.md)**