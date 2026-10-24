---
id: annotate-public-signatures
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [annotate-only-where-needed]
---

# Annotate the signatures of public functions

Annotate parameter and return types of functions that other modules call, and leave local code and lambdas to inference. A public signature is a contract: with annotations, a change in the implementation fails at the definition instead of silently changing the type for every caller.

## Check

1. For every new or changed function used outside its module: are its parameters and return type annotated, or stated in a signature file?
2. Are local functions and lambdas annotated without need?

## Report

The missing or needless annotations.

## Do not report

- Functions whose signature a `.fsi` file states.
- Scripts.

## Examples (illustrative, not exhaustive)

```fsharp
let parse text = // finding: public, signature inferred
    Order.tryParse text |> Result.mapError ParseFailed

// correction
let parse (text: string) : Result<Order, ImportError> =
    Order.tryParse text |> Result.mapError ParseFailed
```

No finding: an unannotated helper nested inside the function that uses it.
