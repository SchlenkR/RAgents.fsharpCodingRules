---
id: classes-for-configured-logic
category: design
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [modules-for-logic]
---

# Group configured operations in immutable classes

Use a small immutable class when its constructor fixes settings that a cohesive group of operations shares. Callers configure the group once and reuse it, instead of passing the same settings to every function.

## Check

1. Do several related module functions receive the same immutable settings on every call?
2. Would a class group them without mutable state or hidden effects?

## Report

The repeated settings parameters and the immutable class that holds them.

## Do not report

- Transformations without settings, and data-only records or unions.
- Classes that exist only to satisfy a dependency injection container.

## Examples (illustrative, not exhaustive)

```fsharp
module Price =
    let gross (taxRate: decimal) (net: decimal) = net * (1m + taxRate) // finding: taxRate passed everywhere
    let tax (taxRate: decimal) (net: decimal) = net * taxRate

// correction
type Prices(taxRate: decimal) =
    member _.Gross(net: decimal) = net * (1m + taxRate)
    member _.Tax(net: decimal) = net * taxRate
```

No finding: a single pure function in a module that takes its rate as a parameter.
