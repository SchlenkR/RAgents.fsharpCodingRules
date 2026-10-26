---
id: classes-for-configured-logic
category: design
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [modules-for-logic]
---

# Group configured operations in immutable classes

Use a small immutable class when its constructor fixes settings that a cohesive group of operations shares and callers reuse that configuration.

## Look for

Look for related module functions repeatedly receiving the same settings. Inspect the calls to establish whether a configured group exists:

```fsharp
module Price =
    let gross taxRate net = net * (1m + taxRate)
    let tax taxRate net = net * taxRate

let quote taxRate net =
    Price.gross taxRate net, Price.tax taxRate net
```

The repeated `taxRate` is a candidate; its name alone does not establish shared configuration.

## Check the context

Trace the settings through callers. Are they fixed for a cohesive set of operations, or do individual calls intentionally vary them? Inspect initialization and effects: constructing the proposed class must not cache changing configuration, open resources, or conceal work. Check whether the operations really belong together rather than merely sharing one parameter type.

## Report when

Report repeated configuration threading when callers can configure the operations once and reuse them without changing behavior. This selected preference groups that stable context in an immutable class. Name the actual shared settings and operations; two functions accepting decimals are insufficient evidence.

## Suggest

Capture stable configuration in the constructor and keep operation inputs explicit:

```fsharp
type Prices(taxRate: decimal) =
    member _.Gross(net: decimal) = net * (1m + taxRate)
    member _.Tax(net: decimal) = net * taxRate

let quote (prices: Prices) net = prices.Gross net, prices.Tax net
```

Construct `Prices` where the caller selects the rate, then reuse it for the intended configuration lifetime.

## Do not report

Keep a single calculation as a function:

```fsharp
let gross taxRate net = net * (1m + taxRate)

let compareRates net = gross 0.07m net, gross 0.19m net
```

These callers deliberately vary the rate; there is no shared configured group. Also exclude transformations without settings, data-only records or unions, and requests to introduce classes solely for a dependency injection container. This rule is an alternative to rule `modules-for-logic`.
