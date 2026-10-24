---
id: compose-small-internal-functions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [name-the-data]
---

# Compose short internal transformations directly

Define a short function that only its own module uses as a typed composition or partial application when its steps and input type are evident. A wrapper that only forwards its parameter through two named functions adds a name and a parameter without adding meaning.

## Check

1. Does a function used only inside its module just forward its parameter through at most two named functions?
2. Would the composition keep types readable and evaluate at the same time as the wrapper?

## Report

The forwarding wrapper and its typed composition.

## Do not report

- Functions other modules call, where parameter names help callers.
- Code where composition worsens type inference or debugging.
- Factories whose construction must run again on every call.

## Examples (illustrative, not exhaustive)

```fsharp
let sortedDistinct (values: int list) = values |> List.distinct |> List.sort // finding: forwarding only

// correction
let sortedDistinct: int list -> int list = List.distinct >> List.sort
```

No finding: a function other modules call, with named parameters.
