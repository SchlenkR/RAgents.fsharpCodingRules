---
id: confine-mutation
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#, part 1: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
  - "Microsoft F# style guide, principles: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/"
---

# Confine mutation to the block that builds a value

Use `let mutable` or a mutable collection only inside the block that computes a value, so that only the finished, immutable result leaves it.

## Look for

Look for `let mutable`, mutable domain fields, or mutable collections that remain accessible after construction. Also look for a variable initialized only to be assigned by branches:

```fsharp
let indexById (orders: Order list) =
    let index = System.Collections.Generic.Dictionary<OrderId, Order>()

    for order in orders do
        index[order.id] <- order

    index

let mutable label = ""
if age < 18 then label <- "minor" else label <- "adult"
```

## Check the context

1. Follow the mutable value through returns, stored fields, and escaping closures. Is only the completed immutable value exposed?
2. Identify the owner of intentional state and its required lifetime. Local mutation is not automatically shared mutation.
3. For branch assignment, check that a direct expression has the same evaluation and scope. For collection changes, compare duplicate-key handling and lookup semantics.

## Report when

Report mutation that unnecessarily escapes the construction block or a temporary mutable variable used solely to choose one value. Explain who can change the exposed state, or why assignment is unnecessary. Resource/state owners covered by rule `modules-for-logic` remain legitimate.

## Suggest

Return an immutable result and bind branch results directly:

```fsharp
let indexById (orders: Order list) =
    orders |> List.map (fun order -> order.id, order) |> Map.ofList

let label = if age < 18 then "minor" else "adult"
```

Here both collection constructions retain the last order for a repeated key. Check equality/comparison behavior before suggesting `Map` for a custom key type.

## Do not report

Leave mutation confined to computing a final value:

```fsharp
let count items =
    let mutable count = 0

    for item in items do
        if qualifies item then
            count <- count + 1

    count
```

No mutable storage escapes. Also exclude measured buffer/span requirements and classes that intentionally own state for their lifetime.
