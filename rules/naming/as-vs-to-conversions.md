---
id: as-vs-to-conversions
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use as for views and to for transformations

Name a conversion `asX` only when it changes the view of the same data, such as a cast or a wrapper without copying, and `toX` when it computes new data in another format, unit, or domain. The prefix tells the caller whether work happens and whether the result still shares the original.

## Check

1. For every new function or member named `as...`: does it format, compute, copy, parse, or classify?
2. For every new `to...`: is it only a cast or upcast?
3. Does the name state the target in domain terms (`toIsoDate`) rather than a technical one (`toString2`)?

## Report

The conversion and its correct prefix.

## Do not report

- Names from FSharp.Core and .NET, such as `List.toSeq`, `Seq.cast`, or `ToString`.
- `ofX` constructors in a module, such as `Money.ofCents`; they follow the FSharp.Core convention.

## Examples (illustrative, not exhaustive)

```fsharp
let asIsoDate (date: DateOnly) = date.ToString "yyyy-MM-dd" // finding: formats, so it transforms
let toIsoDate (date: DateOnly) = date.ToString "yyyy-MM-dd" // correction
```

No finding: `let asReadOnly (items: ResizeArray<'a>) = items :> IReadOnlyList<'a>`.
