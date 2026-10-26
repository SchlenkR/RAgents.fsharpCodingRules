---
id: loops-over-compound-folds
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [fold-cohesive-state]
sources:
  - "Don Syme, Notes on learning F#, part 1, why we use fold a lot less: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
---

# Prefer simple reductions over compound folds

Replace a `fold` whose accumulator is a tuple or record, and every `||>` fold, by specific functions such as `sumBy`, `partition`, or `groupBy`, a comprehension, or a `for` loop over a confined `let mutable`.

## Look for

Look for `fold` or `foldBack` threading a tuple, record, or anonymous record, and for reductions written with `||>`:

```fsharp
let count, total =
    ((0, 0m), invoices)
    ||> List.fold (fun (count, total) invoice -> count + 1, total + invoice.amount)
```

Trace named accumulator types too; the shape may be hidden behind a helper.

## Check the context

1. Identify the result each accumulator field computes. Is there a direct `sumBy`, `partition`, `groupBy`, `choose`, `countBy`, or comprehension?
2. Check whether the input is a stored collection or an effectful/one-shot sequence before splitting one traversal into several.
3. Compare update order, exception behavior, and numeric operations. If separate reductions repeat work or effects, keep a single pass with confined mutation.

## Report when

Report compound folds or `||>` reductions that can be expressed by direct operations or a straightforward loop under this selected preference. Explain which operation the accumulator obscures. This position is alternative to rule `fold-cohesive-state`; it does not prohibit every state transition expressed as a fold.

## Suggest

For the in-memory invoice list, use its count and the named total operation:

```fsharp
let count = invoices.Length
let total = invoices |> List.sumBy _.amount
```

For a stream or interdependent updates, propose one `for` loop with mutation confined to the result-building block. Preserve one-pass effects instead of mechanically duplicating enumeration.

## Do not report

Leave a simple domain state transition unchanged:

```fsharp
let account = events |> List.fold Account.apply Account.empty
```

The accumulator is the named account state and each event applies one transition. Also exclude recursive tree traversal; a loop over a flat collection does not necessarily replace its structure.
