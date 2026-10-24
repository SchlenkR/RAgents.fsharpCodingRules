---
id: fail-or-degrade-per-datum
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Decide per kind of data whether a failure stops, drops, or degrades

Decide for each kind of loaded data whether a failure stops the operation, drops the item with a warning, or falls back to a default, and make that decision in the workflow. One policy for everything either lets a broken thumbnail fail a checkout or lets a broken price pass silently.

## Check

1. Does one invalid element stop an operation that could show the others, such as search results?
2. Is data that a decision depends on, such as a price or a permission, dropped or defaulted instead of stopping the operation?
3. Is every dropped element and every default logged?

## Report

The data, the policy that fits it, and a missing log.

## Do not report

- Failures that the caller handles explicitly.

## Examples (illustrative, not exhaustive)

```fsharp
let toHits rows = // finding: one bad row fails the whole result page
    rows
    |> List.map (fun row ->
        match SearchHit.tryCreate row with
        | Ok hit -> hit
        | Error error -> failwith error)

// correction
let toHits (warn: string -> unit) rows =
    [
        for row in rows do
            match SearchHit.tryCreate row with
            | Ok hit -> hit
            | Error error -> warn $"Dropped search hit: {error}"
    ]
```

No finding: a checkout that stops because the price of an item cannot be loaded.
