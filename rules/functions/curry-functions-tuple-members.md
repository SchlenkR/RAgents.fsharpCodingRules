---
id: curry-functions-tuple-members
category: functions
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Don Syme, On currying: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/on-currying.md"
  - "Microsoft F# component design guidelines: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
---

# Curry module functions and tuple members

Give functions in modules curried parameters, ordered from the least to the most varying with the piped data last, and give members of types that .NET code calls tupled parameters.

## Look for

Look for module functions taking several independently meaningful arguments as one tuple, or putting the normally piped input before configuration. Inspect C#-facing members for curried argument groups:

```fsharp
let addDays (days: int, date: System.DateOnly) = date.AddDays days

let due = addDays (30, invoiceDate)
```

## Check the context

1. Inspect call sites: are arguments independently partially applied, and which value is naturally piped?
2. Distinguish separate arguments from one tuple-valued datum, such as a coordinate.
3. Check C# consumers, delegates, overloads, and published signatures before changing calling convention. Update callers together with an internal signature.

## Report when

Report tupled module arguments that represent separate inputs to F# logic, a parameter order that prevents the intended pipeline, or curried members intended for ordinary C# calls. Curried module functions permit partial application; tupled members fit .NET call conventions. Identify the actual consumer that benefits.

## Suggest

Curry independent function parameters with the main input last:

```fsharp
let addDays (days: int) (date: System.DateOnly) = date.AddDays days
let due = invoiceDate |> addDays 30
```

For C#-facing members, use one tupled argument group. Treat published signature changes as API changes rather than presenting them as a local syntax edit.

## Do not report

Leave a tuple representing one datum unchanged:

```fsharp
let distanceFromOrigin (x, y) = sqrt (x * x + y * y)
```

The coordinate is one input. Also leave tupled members called from C#, and functions or lambdas constrained by a .NET delegate signature.
