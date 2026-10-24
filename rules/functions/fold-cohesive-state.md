---
id: fold-cohesive-state
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [loops-over-compound-folds]
---

# Fold cohesive state without mutation

Write a reduction whose state belongs together, including a tuple or record accumulator, as a pure fold instead of a loop over mutable variables. One explicit accumulator keeps related updates together, and the state transition reads as one expression.

## Check

1. Does a loop mutate only accumulator variables, once per input element?
2. Can a small fold express the same result without traversing the input several times?

## Report

The accumulator loop and its fold.

## Do not report

- Reductions that a standard function already expresses, such as `List.sumBy`.
- Measured performance requirements that justify mutable buffers.

## Examples (illustrative, not exhaustive)

```fsharp
let totals (amounts: decimal list) =
    let mutable count = 0
    let mutable amount = 0m

    for value in amounts do
        count <- count + 1
        amount <- amount + value // finding: a cohesive reduction over mutable variables

    { count = count; amount = amount }

// correction
let totals (amounts: decimal list) =
    amounts
    |> List.fold
        (fun state value -> { count = state.count + 1; amount = state.amount + value })
        { count = 0; amount = 0m }
```

No finding: `List.sumBy _.amount` for one independent total.
