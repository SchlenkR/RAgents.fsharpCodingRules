---
id: name-the-cases
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name the cases instead of using bool, Choice, or tuples

Represent a distinction between domain cases as a union with named cases, also for only two cases, instead of a `bool` parameter, `Choice<_, _>`, or a tuple whose parts differ only by position. `true`, `Choice2Of2`, and `snd` say nothing at the call site; a named case makes every call and every match readable.

## Check

1. For every new `bool` parameter: does it select between two behaviors, so that callers pass a bare `true` or `false`?
2. Look for `Choice` in signatures and for tuples such as `string * string` whose parts have different meanings.
3. Look for functions returning `bool * 'a` for found or not found.

## Report

The parameter or type and the union with named cases, or `option` for found or not found.

## Do not report

- A `bool` that stores a yes/no fact as data, such as `isActive` in a record.
- Predicates that return `bool`, such as `isOverdue`.
- .NET `TryParse` and `TryGetValue` patterns at the boundary.

## Examples (illustrative, not exhaustive)

```fsharp
let export (includeArchived: bool) orders = ... // finding: callers write export true orders

// correction
[<RequireQualifiedAccess>]
type Archived =
    | Include
    | Exclude

let export (archived: Archived) orders = ...
```

No finding: `let isOverdue today invoice = invoice.dueDate < today`.
