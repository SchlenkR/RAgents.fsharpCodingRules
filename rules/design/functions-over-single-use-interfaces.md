---
id: functions-over-single-use-interfaces
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Microsoft F# component design guidelines, use interfaces to group related operations: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
  - "Mark Seemann, Partial application is dependency injection: https://blog.ploeh.dk/2017/01/30/partial-application-is-dependency-injection/"
---

# Pass a function or a value instead of a single-use interface

Take a dependency as a function or the value it produces instead of defining an interface for one operation and one implementation.

## Look for

Look for one-operation interfaces introduced for injection or mocking, and interfaces mirroring one concrete class. Inspect implementation and use together:

```fsharp
type IClock =
    abstract Now: unit -> System.DateTimeOffset

type SystemClock() =
    interface IClock with
        member _.Now() = System.DateTimeOffset.UtcNow

let isExpired (clock: IClock) expiresAt = expiresAt < clock.Now()

let hasExpired expiresAt = isExpired (SystemClock()) expiresAt
```

## Check the context

Find all implementations, including object expressions, test doubles, external contracts, and framework registrations. Count cohesive operations rather than just declared members. Trace when and how often callers need the operation: a value can replace one observation, but repeated or deferred observations need a function. Check public API compatibility before removing a type.

## Report when

Report a locally controlled interface that only wraps one operation and has no required object contract, cohesive operation group, or second real implementation. Identify the actual seam and how its caller can express it as data or a function. A test mock alone does not require an interface because a function parameter is also replaceable.

## Suggest

Pass the already available value when one observation is sufficient:

```fsharp
let isExpired (now: System.DateTimeOffset) expiresAt = expiresAt < now

let hasExpired expiresAt =
    isExpired System.DateTimeOffset.UtcNow expiresAt
```

Use `unit -> System.DateTimeOffset` instead when evaluation must remain deferred or repeated. Update composition sites without changing the number or timing of observations. For several related injected operations, consider a cohesive record of functions rather than introducing unrelated parameters.

## Do not report

Keep interfaces with meaningful grouped operations and real alternative implementations:

```fsharp
type IBlobStore =
    abstract Read: string -> byte array
    abstract Write: string * byte array -> unit
    abstract Delete: string -> unit

let readFrom (store: IBlobStore) key = store.Read key
```

If project inspection finds disk and remote implementations, this is a real contract. Also retain interfaces required by .NET or a framework, such as `IDisposable` and `IComparable`.
