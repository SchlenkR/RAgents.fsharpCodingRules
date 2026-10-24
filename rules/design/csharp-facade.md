---
id: csharp-facade
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Microsoft F# component design guidelines, guidelines for libraries for use from other .NET languages: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
---

# Keep F# APIs idiomatic and add a facade for C#

Write APIs that F# code calls with F# types, such as lists, options, function types, and unions, and when C# code consumes the library, add a separate facade that converts to .NET types. Bending the core towards C# makes every F# caller pay for interop, while F# types still leak awkwardly into C#.

## Check

1. For every new public F# signature: does it use `Func`, `Action`, `IReadOnlyList`, nullable, or `out` parameters although its callers are F# code?
2. Does a type that C# code calls expose `FSharpList`, `FSharpOption`, `FSharpFunc`, or curried members?
3. Are the conversions in one facade module or class, not spread through the core?

## Report

The signature and where its conversion belongs.

## Do not report

- Libraries designed for C# first, which follow .NET conventions throughout.
- `Task` and `seq` in F# APIs; both are idiomatic on both sides.
- Collection interfaces chosen for indexing or ownership, not for C# callers.

## Examples (illustrative, not exhaustive)

```fsharp
let applyDiscounts (isEligible: Func<Customer, bool>) (orders: IReadOnlyList<Order>) = ... // finding

// correction
let applyDiscounts (isEligible: Customer -> bool) (orders: Order list) = ...

type Discounts =
    static member Apply(isEligible: Func<Customer, bool>, orders: IEnumerable<Order>) =
        applyDiscounts isEligible.Invoke (List.ofSeq orders) |> List.toArray
```

No finding: an F# function that returns `Task<Order list>`.
