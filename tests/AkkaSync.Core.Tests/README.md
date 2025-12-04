# AkkaSync.Core Unit Tests

## Overview

Comprehensive unit test suite for `AkkaSync.Core` covering core abstractions, configurations, and utilities. **All 29 tests passing**.

## Test Coverage

### Configuration Tests (`Configuration/PipelineConfigTests.cs`)
- **BuildLayers_WithNoDependencies_ReturnsSingleLayer**: Verifies layer grouping for independent pipelines
- **BuildLayers_WithLinearDependencies_ReturnsMultipleLayers**: Tests sequential dependency resolution
- **BuildLayers_WithComplexDependencies_ReturnsCorrectLayers**: Validates multi-pipeline convergent dependencies
- **BuildLayers_WithMissingDependency_ThrowsInvalidOperationException**: Ensures error handling for invalid references
- **BuildLayers_WithCyclicDependency_ThrowsInvalidOperationException**: Detects and rejects circular dependencies

### Model Tests
#### TransformContext (`Models/TransformContextTests.cs`)
- **TransformContext_WithValidData_CreatesSuccessfully**: Basic instantiation with required fields
- **TransformContext_WithMultipleTables_PreservesAllData**: Validates multi-table data preservation

#### SyncHistoryRecord (`Models/SyncHistoryRecordTests.cs`)
- **SyncHistoryRecord_WithDefaults_HasCorrectInitialState**: Verifies default status and timestamps
- **SyncHistoryRecord_WithAllProperties_StoresCorrectly**: Tests full property population
- **SyncHistoryRecord_CanUpdateStatus**: Validates status mutation
- **SyncHistoryRecord_CanUpdateCursor**: Tests cursor progression for resume capability
- **SyncHistoryRecord_SupportsVariousStatuses** (Theory): Parameterized test for Pending/Running/Completed/Failed states

### Abstraction Tests
#### TransformerBase (`Abstractions/TransformerBaseTests.cs`)
- **SingleTransformer_TransformsDataCorrectly**: Basic transformation (UpperCaseTransformer)
- **ChainedTransformers_ApplyInSequence**: Validates Chain of Responsibility pattern (2 transformers)
- **Transformer_PreservesNonStringValues**: Ensures numeric/non-string data passes through unchanged
- **ThreeChainedTransformers_ApplyAllTransforms**: Complex chaining scenario (3 transformers)

Includes test implementations:
- `UpperCaseTransformer`: Converts string values to uppercase
- `PrefixTransformer`: Adds configurable prefix to string values

### Common Utilities Tests (`Common/SyncGeneratorTests.cs`)
- **ComputeSha256_WithSingleValue_GeneratesValidHash**: Validates SHA256 hash format and length
- **ComputeSha256_WithMultipleValues_CombinesAndHashesThem**: Tests comma-delimited concatenation
- **ComputeSha256_WithIdenticalInputs_ProducesSameHash**: Idempotency verification
- **ComputeSha256_WithDifferentInputs_ProducesDifferentHash**: Collision resistance check
- **ComputeSha256_WithEmptyStringsInParams_IgnoresThem**: Whitespace filtering behavior
- **ComputeSha256_WithWhitespaceStrings_TrimsBeforeHashing**: Normalization verification
- **ComputeSha256_WithNoValidValues_ThrowsArgumentException**: Error handling for empty input

### Actor Integration Tests (`Actors/PipelineManagerActorTests.cs`)
- **PipelineManagerActorTestPlaceholder**: Stub for future Akka.TestKit integration tests
  - Note: Full implementation requires comprehensive mock infrastructure for provider registries

## Running the Tests

```bash
# Run all tests
dotnet test tests/AkkaSync.Core.Tests/

# Run with verbosity
dotnet test tests/AkkaSync.Core.Tests/ -v normal

# Run specific test class
dotnet test tests/AkkaSync.Core.Tests/ --filter "ClassName=AkkaSync.Core.Tests.Configuration.PipelineConfigTests"

# Run with coverage
dotnet test tests/AkkaSync.Core.Tests/ /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Results

```
Test run completed successfully.
Failed:     0
Passed:    29
Skipped:    0
Total:     29
Duration:  ~1 second
```

## Dependencies

- **xunit**: 2.8.1 - Test framework
- **xunit.runner.visualstudio**: 2.8.1 - Test explorer integration
- **Akka.TestKit.Xunit2**: 1.5.55 - Akka actor testing utilities
- **Moq**: 4.20.70 - Mocking framework
- **coverlet.collector**: 6.0.0 - Code coverage

## Future Enhancements

1. **Actor Tests**: Complete PipelineManagerActor and SyncWorkerActor tests with full mock infrastructure
2. **Plugin Integration Tests**: Test actual source/sink provider implementations
3. **End-to-End Tests**: Full pipeline execution scenarios with real file I/O and database writes
4. **Performance Tests**: Benchmark transformation chains and batch processing
5. **Concurrency Tests**: Validate multi-pipeline parallel processing and race condition handling

## Code Coverage Goals

- **Current**: Core abstractions and models (~50% of codebase)
- **Target**: >70% coverage for production readiness
- **Future**: Include actors, providers, and integration tests for >80%
