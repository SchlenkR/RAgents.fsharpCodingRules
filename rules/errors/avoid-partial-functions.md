---
id: avoid-partial-functions
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "FSharpLint FL0066 NoPartialFunctions: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0066.md"
---

# Avoid functions that throw on missing data

Use `tryHead`, `tryFind`, `tryItem`, `Map.tryFind`, or a match instead of `List.head`, `List.find`, `Seq.exactlyOne`, `Map.find`, `Option.get`, or `.Value` on an option when the data may be missing. These functions throw a generic exception far from the decision about what missing means.

## Check

1. Look for `head`, `last`, `find`, `pick`, `exactlyOne`, `item`, index access, `Map.find`, `Option.get`, and `.Value` on options.
2. For each: does the code right above prove that the value exists, or can input data make it missing?

## Report

The call and its `try` form with an explicit decision for the missing case.

## Do not report

- Tests, where an exception is the desired failure.
- Access that the same function has just proven safe, such as an index below `Array.length`.

## Examples (illustrative, not exhaustive)

```fsharp
let customer = customers |> List.find (fun c -> c.id = order.customerId) // finding: throws if unknown

// correction
match customers |> List.tryFind (fun c -> c.id = order.customerId) with
| Some customer -> Ok customer
| None -> Error(UnknownCustomer order.customerId)
```

No finding: `List.head` in a test that asserts the first element.
