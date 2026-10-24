---
id: name-the-data
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [compose-small-internal-functions]
sources:
  - "Microsoft F# coding conventions, partial application and point-free programming: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
---

# Name the data instead of composing point-free

Define module-level functions with named parameters and a pipeline, not as point-free compositions with `>>` or partial applications. Point-free definitions lose parameter names in tooltips and signatures and hide intermediate values from error messages and the debugger.

## Check

1. For every new module-level function defined without parameters, such as `let f = g >> h` or `let f = List.map transform`: is it public or used in more than one place?
2. Does a composition chain have more than two steps, so the intermediate types are invisible?

## Report

The definition and its form with a named parameter and a pipeline.

## Do not report

- Short compositions passed as arguments, such as `List.map (Order.total >> Money.format)`.
- Combinator libraries and DSLs, such as parsers, where composition is the API.

## Examples (illustrative, not exhaustive)

```fsharp
let activeNames = // finding
    List.filter isActive >> List.map displayName >> List.distinct >> List.sort

// correction
let activeNames (customers: Customer list) =
    customers
    |> List.filter isActive
    |> List.map displayName
    |> List.distinct
    |> List.sort
```

No finding: `orders |> List.map (Order.total >> Money.format)` inside a function body.
