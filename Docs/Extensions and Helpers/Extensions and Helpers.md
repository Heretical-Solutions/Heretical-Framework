# Extensions and Helpers Module

## Overview

The Extensions and Helpers module provides utility methods, extension methods, and helper classes that simplify common programming tasks throughout the framework. It includes both engine-agnostic utilities and Unity-specific helpers.

## Module Variants

### Extensions and helpers (Base)
- **Location**: `Assets/Scripts/Framework/Extensions and helpers/`
- **Purpose**: Engine-agnostic utility methods and extensions
- **Dependencies**: None (pure C#)

### Extensions and helpers.Unity
- **Location**: `Assets/Scripts/Framework/Extensions and helpers.Unity/`
- **Purpose**: Unity Engine-specific utilities and extensions
- **Dependencies**: Unity Engine

## Core Components

### Address Extensions
- **File**: `AddressExtensions.cs`
- **Purpose**: Extensions for address/identifier manipulation
- **Features**:
  - Address formatting and validation
  - Conversion between address types
  - Address comparison utilities

### Delegate Extensions
- **File**: `DelegateExtensions.cs`
- **Purpose**: Extensions for delegate and event handling
- **Features**:
  - Safe delegate invocation
  - Event subscription/unsubscription helpers
  - Null-safe delegate operations

### Exception Extensions
- **File**: `ExceptionExtensions.cs`
- **Purpose**: Enhanced exception handling and reporting
- **Features**:
  - Exception message formatting
  - Stack trace utilities
  - Exception aggregation helpers

### Math Extensions and Helpers
- **Files**: `MathExtensions.cs`, `MathHelpers.cs`
- **Purpose**: Mathematical operations and utilities
- **Features**:
  - Extended mathematical functions
  - Interpolation methods
  - Numerical comparison with tolerance
  - Vector and matrix operations (engine-agnostic)

### Path Extensions
- **File**: `PathExtensions.cs`
- **Purpose**: File path manipulation utilities
- **Features**:
  - Cross-platform path operations
  - Path validation and normalization
  - Relative path calculations

### Progress Extensions
- **File**: `ProgressExtensions.cs`
- **Purpose**: Progress tracking and reporting utilities
- **Features**:
  - Progress calculation helpers
  - Progress event handling
  - Completion percentage utilities

### Task Extensions
- **File**: `TaskExtensions.cs`
- **Purpose**: Enhanced Task and async operation support
- **Features**:
  - Task continuation helpers
  - Timeout operations
  - Exception handling for async operations
  - Task result conversion utilities

### Type Extensions and Helpers
- **Files**: `TypeExtensions.cs`, `TypeHelpers.cs`
- **Purpose**: Type system utilities and reflection helpers
- **Features**:
  - Type inspection utilities
  - Generic type operations
  - Assembly and namespace helpers
  - Type conversion utilities

### Enumerators
- **Directory**: `Enumerators/`
- **Purpose**: Custom enumeration patterns and utilities
- **Features**:
  - Memory-efficient enumerators
  - Custom iteration patterns
  - LINQ-like operations without allocation

## Unity-Specific Helpers

### Math Helpers Unity
- **File**: `MathHelpersUnity.cs`
- **Purpose**: Unity-specific mathematical operations
- **Features**:
  - Vector2/Vector3/Vector4 utilities
  - Quaternion operations
  - Unity-specific interpolation methods
  - Transform manipulation helpers
  - Bounds and collision utilities

## Common Extension Patterns

### Safe Operations
Many extensions provide null-safe alternatives to common operations:

```csharp
// Safe delegate invocation
action?.SafeInvoke();

// Safe type checking
if (obj.IsOfType<MyClass>())
{
    // Safe casting operations
}
```

### Fluent Interfaces
Extensions support method chaining for improved readability:

```csharp
var result = value
    .Clamp(min, max)
    .Round(2)
    .ConvertTo<float>();
```

### Performance Optimizations
Many helpers provide optimized alternatives to common operations:

```csharp
// Optimized string operations
var hash = text.FastHash();

// Efficient collection operations
var count = collection.CountFast(predicate);
```

## Usage Guidelines

### When to Use Extensions
- **Common Operations**: Frequently used operations across the codebase
- **Readability**: When extension methods improve code readability
- **Type Safety**: When extensions can provide type-safe alternatives
- **Performance**: When optimized implementations are available

### Best Practices

1. **Keep Extensions Focused**: Each extension should have a single, clear purpose
2. **Use Descriptive Names**: Extension method names should be self-documenting
3. **Consider Performance**: Extensions used in hot paths should be optimized
4. **Maintain Null Safety**: Extensions should handle null inputs gracefully
5. **Document Edge Cases**: Complex extensions should document behavior edge cases

### Integration with Framework Modules

Extensions and helpers are used throughout the framework:
- **Collections**: Type extensions for collection operations
- **Persistence**: Path extensions for file operations
- **Messaging**: Delegate extensions for event handling
- **Math Operations**: Math helpers for game calculations

## Examples

### Basic Usage
```csharp
// Math operations
var clamped = value.Clamp(0f, 1f);
var interpolated = start.Lerp(end, t);
var rounded = value.Round(2);

// Type operations
if (obj.IsOfType<IMyInterface>())
{
    var converted = obj.SafeCast<IMyInterface>();
}

// Path operations
var normalized = path.NormalizePath();
var relative = basePath.GetRelativePath(targetPath);
```

### Unity-Specific Usage
```csharp
// Vector operations
var direction = (target - source).Normalized();
var distance = source.DistanceTo(target);

// Transform utilities
transform.SetPositionX(newX);
transform.LookAtSmooth(target, smoothTime);

// Bounds operations
var contains = bounds.ContainsPoint(point);
var expanded = bounds.Expand(margin);
```

### Async Operations
```csharp
// Task extensions
await operation.WithTimeout(TimeSpan.FromSeconds(5));
var result = await task.OnCompleted(onSuccess, onError);

// Progress tracking
await longOperation.TrackProgress(progressCallback);
```

## Performance Considerations

### Hot Path Optimizations
- Extensions used in game loops are optimized for performance
- Avoid boxing/unboxing in generic extensions
- Use struct-based operations where possible

### Memory Efficiency
- Extensions avoid unnecessary allocations
- String operations use StringBuilder when appropriate
- Collection operations prefer in-place modifications

### Unity-Specific Optimizations
- Vector operations use Unity's built-in SIMD when available
- Transform operations batch matrix calculations
- Physics operations use Unity's collision detection optimizations

## Related Documentation

- [Math Helpers](./Math%20Helpers/Math%20Helpers.md) - Detailed mathematical operations
- [Type Helpers](./Type%20Helpers/Type%20Helpers.md) - Type system utilities
- [Asynchronous](../Asynchronous/Asynchronous.md) - Async operation patterns