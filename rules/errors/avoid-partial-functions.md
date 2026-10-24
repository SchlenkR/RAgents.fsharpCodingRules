---
id: avoid-partial-functions
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "FSharpLint FL0066 NoPartialFunctions: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0066.md"
---

# Avoid functions that throw on missing data

Use `tryHead`, `tryFind`, `tryItem`, `tryExactlyOne`, `Map.tryFind`, or a match instead of `List.head`, `List.find`, `Seq.exactlyOne`, `Map.find`, `Option.get`, or `.Value` on an option when the input can break their preconditions. These functions throw a generic exception far from the decision about what a missing or ambiguous value means.

## Check

1. Look for `head`, `last`, `find`, `pick`, `exactlyOne`, `item`, index access, `Map.find`, `Option.get`, and `.Value` on options.
2. Does nearby code prove every precondition: a non-empty input, an existing key, `0 <= index && index < length`, exactly one element for `exactlyOne`?

## Report

The call and its `try` form with an explicit decision for the missing case.

## Do not report

- Tests, where an exception is the desired failure.
- Access whose complete preconditions the same function checks right before it.

## Examples (illustrative, not exhaustive)

```fsharp
let customer = customers |> List.find (fun c -> c.id = order.customerId) // finding: throws if unknown

// correction
match customers |> List.tryFind (fun c -> c.id = order.customerId) with
| Some customer -> Ok customer
| None -> Error(UnknownCustomer order.customerId)
```

No finding: `List.head` in a test that asserts the first element.
