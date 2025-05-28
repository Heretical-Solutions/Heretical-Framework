# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a sophisticated Unity 2021.3.25f1 LTS project implementing a comprehensive game development framework called "Heretical Unity Projects". The framework emphasizes performance, modularity, and data-oriented design patterns.

## Key Build Commands

**Unity Standard Build Process:**
- Use Unity Editor for building - no custom build scripts present
- Project uses Universal Render Pipeline (URP)
- Linux build support is configured via Unity's toolchain

**Testing:**
- Use Unity Test Runner for running tests
- Edit mode tests: Window → General → Test Runner → EditMode
- Play mode tests: Window → General → Test Runner → PlayMode

**Package Management:**
- NuGet packages managed via NuGetForUnity (Window → NuGet → Manage NuGet Packages)
- Git-based Unity packages in Packages/manifest.json
- NuGet packages stored in Assets/Plugins/NuGet/Packages/

## High-Level Architecture

### Core Framework Structure (Assets/Scripts/Framework/)

**Entity Component System:**
- Built on DefaultECS with multi-world architecture
- **Registry World**: Cross-world entity tracking and ID management
- **Simulation World**: Game logic execution
- **View World**: Visual representation and rendering
- **Event World**: Event storage and cross-world communication
- Entity sharing between worlds using SharedEntityID system

**Memory Management Strategy:**
- **Allocations System**: Command pattern for resource allocation with descriptors
- **Collections**: Multiple variants (standard, concurrent, non-allocating, unmanaged)
- **Object Pools**: Comprehensive pooling with decorator patterns (timers, cleanup, variants)
- Emphasis on non-allocating variants for performance-critical systems

**Communication Patterns:**
- **Messaging**: Event bus system with allocating/non-allocating variants
- **Delegates & Subscriptions**: Publisher-subscriber with concurrent support  
- **MVVM**: Model-View-ViewModel for UI binding and data presentation
- **Blackboard**: Shared data storage with Unity MonoBehaviour integration

**State Management:**
- **FSM**: Finite state machines (regular, concurrent, non-allocating variants)
- **Persistence**: Multiple serialization formats (JSON, XML, CSV, YAML, Protobuf, Binary)
- Data-oriented design with serializable, cache-friendly structures

**Utility Systems:**
- **Time Management**: Timer systems, time updaters, delta time handling
- **Dependency Injection**: Autofac and Zenject integration
- **Logging**: Structured logging with Unity Editor integration
- **Networking**: LiteNetLib integration for multiplayer

### Key Third-Party Dependencies

**Critical NuGet Packages:**
- DefaultEcs (0.17.2) - Core ECS framework
- Autofac (8.3.0) - Dependency injection container
- LiteNetLib (1.2.0) - Networking foundation
- JoltPhysicsSharp (2.9.13) - Physics engine
- protobuf-net (3.2.45) - High-performance serialization

**Unity Packages:**
- Extenject (Zenject fork) - Dependency injection for Unity
- Addressables - Asset management
- Cinemachine - Camera system
- URP - Universal Render Pipeline

### Folder Structure Patterns

The framework uses a hierarchical namespace approach:
- `Module.SubModule.Feature` folder structure
- Corresponding C# namespaces match folder hierarchy
- Separate assemblies defined via .asmdef files
- "Core" assembly contains the main framework (Framework.asmdef)

### Sample Integration

Comprehensive samples demonstrate framework usage:
- ECS character controller implementation
- Message bus patterns (allocating/non-allocating)
- Object pooling with timers and variants
- State machine usage patterns
- Persistence and serialization examples
- Async/sync system execution

## Development Guidelines

**Assembly References:**
- Main framework in "Core" assembly (allows unsafe code)
- Modular assembly structure for different subsystems
- Heavy use of interfaces for decoupling

**Performance Considerations:**
- Prefer non-allocating variants for runtime systems
- Use concurrent collections where thread safety needed
- Leverage object pooling for frequently created/destroyed objects
- Data-oriented design patterns throughout

**Architecture Patterns:**
- Separation of concerns via multi-world ECS
- Factory patterns for system creation
- Decorator patterns for extending functionality
- Command patterns for resource allocation
- Visitor patterns for serialization

## Branch Information

- **Main branch**: develop (use for PRs)
- **Current status**: Clean working directory
- Git-based project with standard Unity .gitignore patterns