---
id: fold-cohesive-state
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [loops-over-compound-folds]
---

# Fold cohesive state without mutation

Write a reduction whose state belongs together, including a tuple or record accumulator, as a pure fold instead of a loop over mutable variables.

## Look for

Look for a loop whose mutable variables are updated together once for each input, forming one small accumulated result:

```fsharp
let totals (amounts: decimal list) =
    let mutable count = 0
    let mutable amount = 0m

    for value in amounts do
        count <- count + 1
        amount <- amount + value

    { count = count; amount = amount }
```

## Check the context

1. Identify the complete accumulator and verify that each update depends only on its previous state and the current element.
2. Check for breaks, externally visible effects, and mutation that must occur in a particular order. Preserve the accepted collection type when choosing `List.fold`, `Array.fold`, or `Seq.fold`.
3. Prefer a standard reduction if it already expresses the whole result. Check performance constraints before allocating a new record for every element.

## Report when

Report a small cohesive accumulation that can become one pure state transition with the same traversal and results. This is the selected preference opposite rule `loops-over-compound-folds`. Show which variables belong together; do not introduce a tuple merely because several unrelated totals are nearby.

## Suggest

Represent the cohesive update as one fold:

```fsharp
let totals (amounts: decimal list) =
    amounts
    |> List.fold
        (fun state value -> { count = state.count + 1; amount = state.amount + value })
        { count = 0; amount = 0m }
```

Retain the original order of element processing and numeric operations. Keep measured mutable-buffer implementations when the allocation cost matters.

## Do not report

Leave a reduction already expressed by the standard function:

```fsharp
let total (lines: InvoiceLine list) = lines |> List.sumBy _.amount
```

A handwritten fold would add an accumulator without clarifying the operation. Also exclude loops whose main purpose is effects rather than computing cohesive state.
