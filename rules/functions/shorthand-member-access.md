---
id: shorthand-member-access
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Access members without a lambda

Write a lambda that only reads a member as `_.Name`, available since F# 8, and read a member of a single value directly instead of piping the value into a lambda.

## Look for

Look for lambdas that only read a member or chain of members, and single-value pipelines into such lambdas:

```fsharp
let names = customers |> List.map (fun customer -> customer.name)
let name = customer |> fun value -> value.name
```

The candidate ends at a member access; comparisons or computations are different shapes.

## Check the context

1. Check the project's F# language version supports shorthand member access (F# 8 or later).
2. Resolve the receiver type and confirm that the lambda only projects members, without extra arguments, effects, or branching.
3. Check that the shorthand still resolves the same member and that direct access retains evaluation order for a single receiver.

## Report when

Report a redundant lambda parameter for a pure member-access chain, or a pipeline that only reads a member of its single input. The shorthand or direct access states that no further work happens. Identify which member chain replaces the lambda; do not rewrite lambdas that calculate a result.

## Suggest

Use shorthand when passing a projection, and direct access for one value:

```fsharp
let names = customers |> List.map _.name
let name = customer.name
```

The same applies to a chain such as `_.address.city` when the receiver type is known. Retain annotations needed for member resolution.

## Do not report

Leave a lambda performing a comparison:

```fsharp
let expensive = orders |> List.filter (fun order -> order.total > 100m)
```

Its body expresses a predicate, not only member access. Also exclude projects on an older F# language version and cases where the shorthand cannot resolve the intended member.
