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

Define module-level functions with named parameters and a pipeline, not as point-free compositions with `>>` or partial applications.

## Look for

Look for module-level functions defined through composition or partial application without naming their input, especially exported/reused functions and chains of more than two transformations:

```fsharp
let activeNames =
    List.filter isActive >> List.map displayName >> List.distinct >> List.sort
```

## Check the context

1. Inspect consumers and signatures: would named inputs clarify the contract or debugging at this boundary?
2. Resolve the intermediate operations so the replacement preserves their order and inferred types.
3. Check when partially applied arguments are evaluated. Turning a captured settings value or a factory result into a function body may re-evaluate it per call.

## Report when

Report point-free module definitions under this selected preference when named parameters expose the input and transformation steps. Long chains conceal intermediate types; public/reused definitions also lose useful parameter names. This rule is an alternative to rule `compose-small-internal-functions`, rather than an additional requirement to apply simultaneously.

## Suggest

Introduce the input and a pipeline:

```fsharp
let activeNames (customers: Customer list) =
    customers
    |> List.filter isActive
    |> List.map displayName
    |> List.distinct
    |> List.sort
```

Keep values that were deliberately computed once in a binding outside the function. Naming the input should not change the lifetime of captured dependencies.

## Do not report

Leave a short composition supplied directly as an argument:

```fsharp
let labels = orders |> List.map (Order.total >> Money.format)
```

The surrounding call supplies the data and the two operations state the projection. Also exclude parser/DSL combinator APIs where composition is the established language of the API.
