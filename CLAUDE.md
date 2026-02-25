# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

RailwayFx is a C# library providing pragmatic functional programming effects (railway-oriented programming). The goal is a good trade-off between maintainability and performance. Targets .NET 10 with nullable reference types enabled.

## Build & Test Commands

```bash
dotnet build                              # Build the entire solution
dotnet test                               # Run all tests
dotnet test --filter "FullyQualifiedName~TestName"  # Run a single test
dotnet test --filter "FullyQualifiedName~ClassName"  # Run tests in a class
```

## Architecture

- **`src/`** — Main library (`RailwayFx`), .NET 10 class library
- **`tests/`** — Unit tests (`RailwayFx.Tests`), NUnit 4 with NUnit3TestAdapter

Solution file: `RailwayFx.slnx` (new XML-based solution format).

## Design Principles

- Favor `struct`-based types and zero-allocation patterns where possible for performance
- Keep the API surface pragmatic — idiomatic C# over pure FP dogma
- Nullable reference types are enabled; leverage the type system to prevent misuse at compile time
- Target latest C# language features (`LangVersion latest` in tests)
