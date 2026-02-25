# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RailwayFx is a C# library providing railway-oriented programming primitives (`Result<T>`, `Error`). The goal is a good trade-off between maintainability and performance, with idiomatic C# over pure FP dogma.

## Build & Test Commands

```bash
dotnet build                              # Build the entire solution
dotnet test                               # Run all tests
dotnet test --filter "FullyQualifiedName~TestName"  # Run a single test
dotnet test --filter "FullyQualifiedName~ClassName"  # Run tests in a class
```

## Architecture

- **`src/`** — Main library (`RailwayFx`), .NET 10 class library
  - `Error.cs` — Base `Error` record (Key, Message), designed for inheritance
  - `Result.cs` — `Result<TValue>` type with Match, equality, implicit conversions
  - `ResultExtensions.cs` — Sync extensions: Map, Bind, Tap, LINQ (Select/SelectMany), collection helpers (Values, Errors, SeparateResults), RInvoke, ThrowOnError
  - `ResultExtensionsAsync.cs` — Async extensions: MatchAsync, MapAsync, BindAsync, TapAsync — each available on both `Result<T>` (sync→async entry point) and `Task<Result<T>>` (pipeline chaining)
- **`tests/`** — Unit tests (`RailwayFx.Tests`), NUnit 4 with NUnit3TestAdapter
  - `ResultTests.cs` — Core type: construction, equality, operators, implicit conversions, guards, ToString
  - `ResultExtensionTests.cs` — Sync extensions: Map, Bind, Tap, LINQ, collections, RInvoke, ThrowOnError
  - `ResultExtensionsAsyncTest.cs` — Async extensions: all overloads on both `Result<T>` and `Task<Result<T>>`

Solution file: `RailwayFx.slnx` (XML-based solution format).

## Design Decisions

- `Result<T>` is a **class**, not a struct — simplifies equality with inheritance, avoids default-value pitfalls
- `Result<T?>` is valid — `null` is a legitimate success value for nullable type parameters. No runtime guard on `Ok()`, relies on compiler nullable analysis
- `Err()` has a runtime guard — `ArgumentNullException` if `null` error is passed
- Async extensions avoid allocations on error paths: sync return via implicit conversion, no `Task.FromResult` wrappers or intermediate state machines
- Sync `Map`/`Bind` use direct `if` branching instead of delegating through `Match` to avoid lambda allocations
- Sync `Tap` success-only is inlined, not delegated to the two-callback overload, to avoid delegate allocation
- Extensions on `object` are avoided (e.g., `RCast` was removed) to keep IntelliSense clean
- Async extensions do not take `CancellationToken` — the caller controls cancellation via closures in callbacks
