---
id: fail-or-degrade-per-datum
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Decide per kind of data whether a failure stops, drops, or degrades

Decide for each kind of loaded data whether a failure stops the operation, drops the item with a warning, or falls back to a default, and make that decision in the workflow. One policy for everything either lets a broken thumbnail fail a checkout or lets a broken price pass silently.


## Look for

Look for one failure policy applied to unlike data: failing a whole list on one optional row, silently dropping decision-critical input, or defaulting every failed load.

```fsharp
let toHits rows =
    rows
    |> List.map (fun row ->
        match SearchHit.tryCreate row with
        | Ok hit -> hit
        | Error error -> failwith error)
```

## Check the context

1. Inspect what the workflow uses the data for. Does correctness depend on it, or can the user still benefit from the remaining items?
2. Find the product or caller contract for partial results and defaults; an image and a price need different policies.
3. Follow rejection and fallback paths to their warning or explicit caller-visible failure. Check that dropped items and substitutions remain observable.

## Report when

Report a stop, drop, or fallback policy that contradicts the data's role, or a silent drop/default. State the affected datum and consequence: an optional hit can disappear with a warning, while an unknown checkout price must prevent the decision. Do not infer that partial results are acceptable solely because the input is a list.

## Suggest

For a search contract that permits partial results, keep valid hits and warn about rejected ones:

```fsharp
let toHits (warn: string -> unit) rows =
    [
        for row in rows do
            match SearchHit.tryCreate row with
            | Ok hit -> yield hit
            | Error error -> warn $"Dropped search hit: {error}"
    ]
```

This intentionally changes the failure policy. For required data, stop with the workflow's explicit failure instead; do not copy the search policy into checkout or permission checks.

## Do not report

Keep a checkout that refuses to continue without a valid price:

```fsharp
match tryLoadPrice productId with
| Ok price -> placeOrder productId price
| Error error -> Error(PriceUnavailable error)
```

The caller receives an explicit failure and no guessed price reaches the order. Also keep failures a caller already handles with a deliberate policy appropriate to that datum.
