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

Take a dependency as a function parameter, or better as the value it would produce, instead of defining an interface, unless several operations belong together or two real implementations exist. A one-member interface with one implementation adds a type, a class, and a mock for what a function type already says.

## Check

1. For every new interface: how many members does it have, and how many implementations outside tests?
2. Was it introduced only to inject or mock one operation?
3. Look for interfaces that mirror one class (`IOrderService` next to `OrderService`).

## Report

The interface and the function parameter, value, or record of functions that replaces it.

## Do not report

- Interfaces that .NET or a framework requires, such as `IDisposable` or `IComparable`.
- A cohesive group of operations with two real implementations, such as file and blob storage.

## Examples (illustrative, not exhaustive)

```fsharp
type IClock = // finding: one member, one implementation
    abstract Now: unit -> DateTimeOffset

let isExpired (clock: IClock) (token: Token) = token.expiresAt < clock.Now()

// correction
let isExpired (now: DateTimeOffset) (token: Token) = token.expiresAt < now
```

No finding: an `IBlobStore` with `read`, `write`, and `delete`, implemented for disk and for cloud storage.
