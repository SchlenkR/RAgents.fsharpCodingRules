---
id: csharp-facade
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Microsoft F# component design guidelines, guidelines for libraries for use from other .NET languages: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
---

# Keep F# APIs idiomatic and add a facade for C#

Write APIs that F# code calls with F# types, and add a separate facade that converts to .NET types when C# code consumes the library.

## Look for

Look for `Func`, `Action`, nullable values, `out` parameters, or .NET collection contracts threaded through an F# core for interop. Also inspect public signatures actually called from C# for F# options, lists, unions, function types, or curried members:

```fsharp
let eligibleOrders (isEligible: System.Func<Customer, bool>) (orders: System.Collections.Generic.IReadOnlyList<Order>) =
    orders
    |> Seq.filter (fun order -> isEligible.Invoke order.customer)
    |> Seq.toList
```

## Check the context

Find F# and C# callers, signature files, and the intended audience of the API. Determine whether the collection contract was chosen for indexing or ownership rather than interop. Trace existing conversions: a facade should centralize them instead of forcing each F# caller to adapt. Preserve eager versus lazy evaluation and null/error handling at the boundary.

## Report when

Report when F# callers carry interop conversions through domain logic, or C# callers must manipulate F# representation types without an intended F#-specific contract. Identify the affected caller and the conversion that belongs in the facade. This preference does not require redesigning a library built for C# first.

## Suggest

Keep the F# operation idiomatic and expose a tupled .NET entry point:

```fsharp
let eligibleOrders (isEligible: Customer -> bool) (orders: Order list) =
    orders |> List.filter (fun order -> isEligible order.customer)

type Orders =
    static member Eligible
        (isEligible: System.Func<Customer, bool>, orders: System.Collections.Generic.IEnumerable<Order>)
        =
        eligibleOrders isEligible.Invoke (List.ofSeq orders) |> List.toArray
```

Choose explicit facade conversions for options and unions as needed; do not spread them through the core.

## Do not report

Keep contracts idiomatic on both sides:

```fsharp
let loadOrders (load: unit -> System.Threading.Tasks.Task<Order list>) = load ()
```

`Task` itself needs no replacement. `seq` is also idiomatic in F#. Exclude collection interfaces chosen for actual indexing or ownership needs and libraries intentionally following .NET conventions throughout.
