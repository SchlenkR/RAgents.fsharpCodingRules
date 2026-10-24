---
id: one-reason-per-none
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Return a union when None has several reasons

Return a union with one case per outcome instead of an `option` when a lookup can fail for different reasons that callers or logs need to tell apart. A `None` that means unknown, expired, and used up at once loses the reason before anyone can react to it.

## Check

1. For every new function that returns an `option`: how many different conditions produce `None`?
2. Does a caller or a log message need to know which one it was?

## Report

The function and the union that names each outcome.

## Do not report

- Lookups with a single reason for absence, such as a key missing from a map.
- Several reasons that get the same reaction everywhere, with no log that needs them.

## Examples (illustrative, not exhaustive)

```fsharp
let tryFindDiscount (code: string) : Discount option = ... // finding: None for unknown, expired, and used up

// correction
type DiscountLookup =
    | Valid of Discount
    | Unknown
    | Expired of expiredOn: DateOnly
    | UsedUp

let findDiscount (code: string) : DiscountLookup = ...
```

No finding: `Map.tryFind sku prices`, where `None` only means that the SKU has no price.
