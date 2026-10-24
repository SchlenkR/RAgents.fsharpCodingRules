---
id: shorthand-member-access
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Access members without a lambda

Write a lambda that only reads a member as `_.Name`, available since F# 8, and read a member of a single value directly instead of piping the value into a lambda. A lambda with an invented parameter name hides that nothing else happens.

## Check

1. Look for `fun x -> x.Name` and chains such as `fun c -> c.Address.City` passed to a function.
2. Look for `value |> fun v -> v.Name` and other pipes into a lambda that only reads a member.

## Report

The lambda and its shorthand or direct access.

## Do not report

- Lambdas that do more than a member access chain, such as a comparison.
- Projects on F# 7 or older.

## Examples (illustrative, not exhaustive)

```fsharp
let names = customers |> List.map (fun c -> c.name) // finding
let name = customer |> fun c -> c.name // finding

// correction
let names = customers |> List.map _.name
let name = customer.name
```

No finding: `orders |> List.filter (fun order -> order.total > 100m)`.
