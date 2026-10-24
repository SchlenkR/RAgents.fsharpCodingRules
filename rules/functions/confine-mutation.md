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

Use `let mutable` or a mutable collection only inside the block that computes a value, so that only the finished, immutable result leaves it. Mutation that escapes to a closure, a field, a return value, or module level becomes shared state that changes under its readers.

## Check

1. For every `let mutable`, `ResizeArray`, `Dictionary`, or `HashSet`: is it reachable after the value is built, because it is returned, stored, or captured by a returned closure?
2. Look for module-level mutable values and mutable record fields in domain types.
3. Is a mutable assigned once by a branch, where an `if` or `match` expression could bind the value directly?

## Report

The escaping or needless mutation and its immutable form.

## Do not report

- Classes that own state with a lifetime (rule `modules-for-logic`).
- Loops measured to need arrays or spans for performance.

## Examples (illustrative, not exhaustive)

```fsharp
let mutable label = "" // finding: assigned once by a branch
if age < 18 then label <- "minor" else label <- "adult"
let label = if age < 18 then "minor" else "adult" // correction

let indexById (orders: Order list) =
    let index = Dictionary<OrderId, Order>()

    for order in orders do
        index[order.id] <- order

    index // finding: callers receive the mutable dictionary

let indexById (orders: Order list) =
    orders |> List.map (fun order -> order.id, order) |> Map.ofList // correction
```

No finding: a `let mutable` counter inside a `for` loop that returns only the final count.
