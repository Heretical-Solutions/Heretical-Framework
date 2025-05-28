# Framework Architecture Overview

## Introduction

The Heretical Unity Projects Framework is a comprehensive, engine-agnostic game development toolkit designed with modularity and flexibility at its core. The framework provides multiple implementation variants to support different environments: Plain Old C# Objects (POCO), Unity Engine integration, and potentially other game engines.

## Core Design Philosophy

### 1. Engine Agnosticism
- **Core Framework**: Engine-independent implementations in base modules
- **Unity Integration**: Optional Unity-specific extensions in `.Unity` folders
- **Modular ECS Support**: DefaultECS integration is optional; framework supports multiple ECS implementations
- **POCO Compatibility**: Full functionality available in plain C# environments

### 2. Performance-First Architecture
- **Multiple Performance Variants**: Most modules provide Standard, Concurrent, NonAlloc, and Async variants
- **Data-Oriented Design**: Emphasis on cache-friendly data structures and separation of data from logic
- **Zero-Allocation Paths**: Critical systems provide non-allocating variants for performance-sensitive scenarios

### 3. Modular Design Principles
- **Interface Segregation**: Heavy use of focused interfaces for maximum flexibility
- **Dependency Inversion**: Extensive dependency injection support (Autofac for POCO, Zenject for Unity)
- **Single Responsibility**: Each module has a clearly defined purpose
- **Open/Closed Principle**: Decorator patterns allow extension without modification

### 4. ECS Framework Flexibility
- **DefaultECS**: One supported ECS implementation with dedicated modules
- **Framework Agnostic**: Core patterns work with any ECS framework
- **Modular ECS Support**: Different ECS frameworks will have separate implementation modules

## Framework Structure

### Naming Convention
The framework uses a hierarchical naming system:
- **Base Module**: Core, engine-agnostic functionality
- **Module.Unity**: Unity Engine-specific implementations
- **Module.Async**: Asynchronous variants
- **Module.Concurrent**: Thread-safe variants
- **Module.NonAlloc**: Zero-allocation variants

### Module Categories

## Core Infrastructure

### Memory and Resource Management
- **[Allocations](./Allocations/Allocations.md)**: Command pattern-based memory allocation strategies
- **[Collections](./Collections/Collections.md)**: High-performance data structures with multiple variants
- **[Object Pools](./Object pools/Object pools.md)**: Comprehensive object pooling system with decorators
- **[Object Bags](./Object Bags/Object Bags.md)**: Dynamic object containers and management

### Communication Systems
- **[Messaging](./Messaging/Messaging.md)**: Event bus and message passing system
- **[Delegates and Subscriptions](./Delegates and subscriptions/DelegatesAndSubscriptions.md)**: Event handling and notification patterns
- **[Blackboard](./Blackboard/Blackboard.md)**: Shared data storage and communication hub

### Data Management
- **[Databases and Repositories](./Databases and repositories/DatabasesAndRepositories.md)**: Repository pattern implementations
- **[Persistence](./Persistence/Persistence.md)**: Multi-format serialization (JSON, XML, CSV, YAML, Protobuf)
- **[Metadata](./Metadata/Metadata.md)**: Dynamic metadata management systems

### State and Time Management
- **[FSM](./State machines/StateMachines.md)**: Finite state machine implementations
- **[Synchronization](./Synchronization/Synchronization.md)**: Coordination and synchronization primitives
- **[Time Management](./Time/Time.md)**: Timer systems and time-based operations

## Engine-Specific Modules

### Unity Integration
- **Asset Import**: Unity asset pipeline management
- **Lifetime Management.Unity**: MonoBehaviour lifecycle integration
- **MVVM.Unity**: Unity UI and UIToolkit bindings
- **IK.Unity**: Unity-specific inverse kinematics

### ECS Framework Support
- **Entities.DefaultECS**: DefaultECS-specific implementations
- **Entities.Networking**: Network-aware entity systems
- Future: Additional ECS framework modules (e.g., Entities.Entitas, Entities.LeoECS)

## Performance Variant Strategy

### When to Use Each Variant

**Standard Variants**
- General-purpose scenarios
- Prototyping and development
- When performance is not critical

**Concurrent Variants**
- Multi-threaded environments
- Shared data access scenarios
- Server-side applications

**NonAlloc Variants**
- Performance-critical game loops
- Mobile/console optimization
- Garbage collection sensitive scenarios

**Async Variants**
- I/O operations
- Long-running computations
- Network operations

## Development Patterns

### Dependency Injection
- **POCO**: Autofac container for pure C# environments
- **Unity**: Zenject integration for Unity-specific scenarios
- **Interface-Based**: All major systems expose interfaces for DI

### Factory Pattern Usage
- Object creation managed through factory patterns
- Runtime configuration and dependency resolution
- Support for multiple implementation variants

### Event-Driven Architecture
- Loose coupling through message passing
- Pluggable system architecture
- Dynamic system composition

### Data-Oriented Design
- Separation of data structures and logic
- Cache-friendly memory layouts
- Serializable data transfer objects (DTOs)

## Cross-Engine Compatibility

### POCO Environment
- Full framework functionality without engine dependencies
- Console applications, services, and standalone tools
- Pure C# dependency injection and configuration

### Unity Integration
- Seamless MonoBehaviour integration
- Unity Editor tooling and inspectors
- Addressable asset system support
- Unity-specific performance optimizations

### Future Engine Support
- Modular design enables easy porting
- Engine-specific modules in separate folders
- Core functionality remains engine-agnostic

## Module Integration Guidelines

### Choosing Implementations
1. Start with engine-agnostic base modules
2. Add engine-specific modules only when needed
3. Use appropriate performance variants based on requirements
4. Prefer composition over inheritance

### Interface-First Development
- Program against interfaces, not implementations
- Enables testing, mocking, and future refactoring
- Supports multiple backend implementations

### Configuration and Bootstrapping
- Use dependency injection for configuration
- Factory pattern for runtime object creation
- Module-specific configuration classes

## Next Steps

For detailed documentation on specific modules, refer to the individual documentation files:
- Core module documentation in respective folders
- Unity-specific integration guides
- Performance optimization recommendations
- Cross-module integration examples