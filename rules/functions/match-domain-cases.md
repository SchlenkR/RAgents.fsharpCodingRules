---
id: match-domain-cases
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Distinguish domain cases with match

Use `match` when code decides between domain cases, also for two cases, instead of `if`/`elif` chains, `.IsCase` properties, or a wildcard over your own union. The compiler checks a match for completeness and reports every place when a case is added; an `if` chain silently takes the `else` branch.

## Check

1. Look for `elif` chains that test the same value or union.
2. Look for `if x.IsSome`, `if result.IsOk`, or `if payment.IsPaid` followed by extracting the value.
3. Look for `| _ ->` over a union defined in this code base, which hides cases added later.

## Report

The chain or wildcard and the exhaustive match.

## Do not report

- `if` for a real condition: a threshold, a guard, an early exit such as `if List.isEmpty lines then ...`.
- Wildcards over open sets: strings, numbers, or unions owned by an external library.

## Examples (illustrative, not exhaustive)

```fsharp
if payment.IsPaid then
    sendReceipt payment // finding: Unpaid and Refunded are skipped silently

// correction
match payment with
| Paid paidAt -> sendReceipt paidAt
| Unpaid
| Refunded -> ()
```

No finding: `if amount > limit then Error AmountTooHigh else Ok amount`.
