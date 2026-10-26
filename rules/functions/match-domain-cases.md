---
id: match-domain-cases
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Distinguish domain cases with match

Use `match` when code decides between domain cases, also for two cases, instead of `if`/`elif` chains or `.IsCase` properties.

## Look for

Look for `if`/`elif` chains testing the same union or domain status, including `.IsSome`, `.IsOk`, and domain case predicates followed by payload access:

```fsharp
if payment.IsPaid then
    sendReceipt payment
```

Inspect the type and predicates before interpreting a property as a union-case check.

## Check the context

1. Establish which domain cases exist and which the conditions distinguish. A boolean guard may be a condition rather than a classification.
2. Trace payload extraction and any additional effects in each branch.
3. Check whether the expression is only an option transformation covered by rule `use-option-combinators`; avoid reporting competing rewrites of the same expression.

## Report when

Report a domain case distinction whose case coverage is hidden in predicates and an implicit `else`. An exhaustive match names the handled and deliberately ignored cases, and adding a case can produce a compiler warning. State which existing cases the conditions currently collect together.

## Suggest

Match the domain cases explicitly while preserving the branches' effects:

```fsharp
match payment with
| Paid _ -> sendReceipt payment
| Unpaid
| Refunded -> ()
```

Group cases that intentionally perform the same work. Do not invent new behavior for previously ignored cases without domain evidence; make the existing policy visible first.

## Do not report

Leave an ordinary threshold or guard as an `if`:

```fsharp
if amount > limit then Error AmountTooHigh else Ok amount
```

The predicate evaluates a condition rather than hiding a finite set of domain cases. Collection-emptiness guards and early exits are likewise permitted. Wildcards inside an existing match belong to rule `no-wildcard-over-own-unions`.
