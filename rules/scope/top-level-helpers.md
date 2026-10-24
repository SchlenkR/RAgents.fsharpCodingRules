---
id: top-level-helpers
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [nest-single-use-bindings]
sources:
  - "Microsoft F# coding conventions, access control: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
---

# Keep helper functions flat at module level

Declare named helper functions at module level, `private` when they are not part of the module's API, instead of nesting them inside their consumer. Flat helpers can be tested and read on their own, and the consumer's body stays short and shallow.

## Check

1. For every function defined with `let` inside another function: is it longer than one line, or does it define further nested functions?
2. Does the nested function work without the consumer's local values, so it can move to module level unchanged?

## Report

The nested helper, corrected to a module-level `let private` function placed directly above its consumer.

## Do not report

- Local values and one-line lambdas such as `let isOpen order = order.closedAt.IsNone`.
- Closures that capture the consumer's parameters or locals; moving them would only add parameters.
- `let` bindings inside computation expressions and list expressions.

## Examples (illustrative, not exhaustive)

```fsharp
let summarize orders =
    let describe order = // finding: three lines, independent of summarize
        let total = order.lines |> List.sumBy _.price
        $"{order.id}: {total}"

    orders |> List.map describe

// correction
let private describe order =
    let total = order.lines |> List.sumBy _.price
    $"{order.id}: {total}"

let summarize orders = orders |> List.map describe
```

No finding: `let isLate order = order.due < today` inside a function that receives `today`.
