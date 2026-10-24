---
id: loops-over-compound-folds
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#, part 1, why we use fold a lot less: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
---

# Prefer simple reductions over compound folds

Replace a `fold` whose accumulator is a tuple or record, and every `||>` fold, by specific functions such as `sumBy`, `partition`, or `groupBy`, a comprehension, or a `for` loop over a confined `let mutable`. The reader of a compound fold has to reconstruct a state machine from one lambda.

## Check

1. Look for `fold` or `foldBack` with a tuple, record, or anonymous record accumulator.
2. Look for `(state, items) ||> List.fold ...`.
3. Could `List.sumBy`, `List.partition`, `List.groupBy`, `List.choose`, `List.countBy`, or a list expression produce the result directly?

## Report

The fold and its replacement by simple reductions, a comprehension, or a confined loop.

## Do not report

- A fold with one simple accumulator, such as applying events to a state.
- Recursive functions that walk a tree.

## Examples (illustrative, not exhaustive)

```fsharp
let count, total = // finding: compound accumulator
    ((0, 0m), invoices)
    ||> List.fold (fun (count, total) invoice -> count + 1, total + invoice.amount)

// correction
let count = invoices.Length
let total = invoices |> List.sumBy _.amount
```

No finding: `events |> List.fold Account.apply Account.empty`.
