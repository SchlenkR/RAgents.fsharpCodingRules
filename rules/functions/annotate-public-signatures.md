---
id: annotate-public-signatures
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [annotate-only-where-needed]
---

# Annotate the signatures of public functions

Annotate parameter and return types of functions that other modules call, and leave local code and lambdas to inference.

## Look for

Look for functions called from another module whose parameter or result types are inferred entirely from the body:

```fsharp
module Orders =
    let parse text =
        Order.tryParse text |> Result.mapError ParseFailed

module Import =
    let row text = Orders.parse text
```

## Check the context

1. Find consumers and establish that the function crosses a module boundary; a top-level binding alone does not prove this.
2. Inspect any matching `.fsi` declaration, which may already specify the contract.
3. Check parameter constraints and the intended result type against the implementation. Separately inspect annotations on local helpers and lambdas for inference needs.

## Report when

Report a missing public contract when a cross-module function has neither explicit parameter/result types nor a signature declaration. Under this selected preference, a body change should fail at the definition instead of changing callers' inferred types. Report local annotations only when they are unnecessary for inference and overload resolution.

## Suggest

State the cross-module contract at the definition:

```fsharp
let parse (text: string) : Result<Order, ImportError> =
    Order.tryParse text |> Result.mapError ParseFailed
```

Use a `.fsi` declaration instead when that is how the module exposes its API. For redundant local annotations, remove only those the build and inferred signature show are unnecessary.

## Do not report

Leave an inferred helper inside its sole consumer:

```fsharp
let nonEmptyNames (names: string list) : string list =
    let notEmpty name = not (System.String.IsNullOrEmpty name)

    names |> List.filter notEmpty
```

The outer function states the contract; local inference supplies the helper type. Also exclude scripts and functions whose signatures are already stated in `.fsi` files.
