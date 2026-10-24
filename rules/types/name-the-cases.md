---
id: name-the-cases
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name the cases instead of using bool or Choice

Represent a distinction between domain cases as a union with named cases, also for only two cases, instead of a `bool` parameter or `Choice<_, _>`, and pass the union itself instead of a `bool` derived from it. `true` and `Choice2Of2` say nothing at the call site; named cases make every call and match readable.

## Check

1. For every new `bool` parameter: does it select a behavior, so that callers pass a bare `true`?
2. Does a caller derive a `bool` from a union just to pass it, such as `isPremium = (plan = Plan.Premium)`?
3. Look for `Choice` in signatures and for functions returning `bool * 'a` for found or not found.

## Report

The parameter or type and the union with named cases, or `option` for found or not found.

## Do not report

- A `bool` storing a yes/no fact as data, such as `isActive` in a record.
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
