# RailwayFx

Railway-oriented programming for C#. A `Result<T>` type that makes error handling explicit, composable, and safe.

## Why

Exceptions are invisible in method signatures. They break composition, make control flow hard to follow, and encourage defensive `try/catch` blocks scattered across the codebase.

`Result<T>` makes the failure path a first-class citizen of your API. Errors are values, not side effects. Code that can fail says so in its return type, and callers must handle both outcomes.

## Quick start

```csharp
using RailwayFx;

// Explicit construction
var success = Result<int>.Ok(42);
var failure = Result<int>.Err(new Error("NOT_FOUND", "User not found"));

// Implicit conversions — less ceremony, same safety
Result<int> value = 42;
Result<int> error = new Error("NOT_FOUND", "User not found");
```

## Composing operations

Chain operations that may fail. If any step produces an error, subsequent steps are skipped and the error propagates through the pipeline.

```csharp
Result<Order> PlaceOrder(string userId) =>
    FindUser(userId)                          // Result<User>
        .Bind(user => ValidateCart(user))     // Result<Cart>
        .Map(cart => cart.ToOrder())           // Result<Order>
        .Tap(order => Log.Info($"Order {order.Id} placed"));
```

### Map

Transforms the success value. Does nothing if the result is an error.

```csharp
Result<string>.Ok("hello").Map(s => s.ToUpper());  // Success["HELLO"]
```

### Bind

Chains an operation that itself returns a `Result<T>`. This is how you compose multiple fallible operations.

```csharp
Result<string>.Ok("42").Bind(s => Parse(s));  // Success[42] or Error
```

### Match

Exhaustive pattern match — forces you to handle both cases.

```csharp
string message = result.Match(
    whenError: err => $"Failed: {err.Message}",
    whenSuccess: value => $"Got: {value}");
```

### Tap

Execute a side effect without altering the result. Useful for logging, metrics, or auditing.

```csharp
result.Tap(
    whenError: err => Log.Warn(err.Message),
    whenSuccess: val => Log.Info($"Processed {val}"));
```

## Async pipelines

All core operations have async counterparts that work on `Task<Result<T>>`, enabling fluent chains without intermediate `await` statements.

```csharp
Result<OrderConfirmation> confirmation = await GetUserAsync(userId)
    .MapAsync(user => EnrichProfileAsync(user))
    .BindAsync(user => PlaceOrderAsync(user))
    .TapAsync(order => SendConfirmationAsync(order));
```

Error paths are optimized: when a result is already in error state, async extensions skip the callback entirely — no `Task.FromResult` wrappers, no unnecessary allocations.

## LINQ support

`Result<T>` implements `Select` and `SelectMany`, so you can use query syntax when combining multiple results.

```csharp
var total = from price in GetPrice(itemId)
            from tax in CalculateTax(price)
            select price + tax;
```

If any step fails, the entire expression short-circuits to the first error.

## Working with collections

```csharp
Result<string>[] results = [Ok("a"), Err(error), Ok("b")];

IEnumerable<string> successes = results.Values();   // ["a", "b"]
IEnumerable<Error> failures  = results.Errors();    // [error]

var (errors, values) = results.SeparateResults();    // both at once
```

## Error types

`Error` is a record with a `Key` and a `Message`. Extend it to carry domain-specific information.

```csharp
public record ValidationError(string Field, string Rule)
    : Error("VALIDATION", $"{Field}: {Rule}");

public record NotFoundError(string Entity, string Id)
    : Error("NOT_FOUND", $"{Entity} '{Id}' not found");
```

## Interop with exceptions

For boundaries where you integrate with code that throws:

```csharp
Result<Config> config = new Func<Config>(() => LoadConfig())
    .RInvoke(ex => new Error("CONFIG_LOAD_FAILED", ex.Message));
```

## Design decisions

- **Class, not struct.** `Result<T>` is a reference type. This simplifies equality semantics with inheritance and avoids default-value pitfalls where a `default(Result<T>)` would silently represent an invalid state.
- **Nullable-aware.** `Result<T?>` is valid — `null` is a legitimate success value when the type parameter is nullable. The library relies on the compiler's nullable reference type analysis rather than runtime guards for value nullability.
- **Async paths avoid unnecessary allocations.** Error short-circuits return synchronously via implicit conversion, without wrapping in `Task.FromResult` or creating intermediate state machines.
- **LINQ support is opt-in by nature.** `Select`/`SelectMany` are extension methods — they only appear when you import `RailwayFx`.

## Requirements

- .NET 10+
- C# with nullable reference types enabled

## License

[MIT](LICENSE)
