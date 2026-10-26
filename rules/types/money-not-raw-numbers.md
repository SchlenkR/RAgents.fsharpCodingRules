---
id: money-not-raw-numbers
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Represent money as a type, not as raw numbers

Carry money between domain operations as decimal amounts with currency, converting to checked minor units only at storage boundaries.

## Look for

Look for `priceCents`, `totalPence`, or bare decimal amounts crossing module boundaries, and unchecked conversions into storage integers:

```fsharp
type Order = { totalCents: int }

let storedCents amount = int (amount * 100m)
```

The domain field carries no currency, and the conversion states neither storage range nor rounding policy.

## Check the context

Trace units, currency, arithmetic, and storage types through callers. Determine the actual column range and domain limits; refunds and prices need not share a positivity rule. Check the currency's minor-unit scale and fractional precision policy. An integer cast can truncate; adding currency metadata alone does not fix an unchecked conversion.

## Report when

Report monetary values crossing domain boundaries without explicit units and currency, or storage conversion that can overflow, silently lose precision, or violate established limits. Name the concrete unit mismatch or missing check. Raw arithmetic confined to one calculation and raw storage row fields are different concerns.

## Suggest

Carry the monetary context and validate before conversion. This example assumes positive EUR/GBP prices, two minor-unit digits, and a signed 32-bit storage column:

```fsharp
type Currency =
    | EUR
    | GBP

type Money = { amount: decimal; currency: Currency }

let tryStorePrice money =
    let maxAmount = decimal System.Int32.MaxValue / 100m

    if money.amount <= 0m || money.amount > maxAmount then
        Error "price is outside the storage range"
    else
        let cents = money.amount * 100m

        if cents <> System.Decimal.Truncate cents then
            Error "price has fractional minor units"
        else
            Ok(int cents)
```

Choose limits, scale, and explicit rounding from the real contract. Convert stored amounts back into domain money at entry.

## Do not report

Keep minor units in the storage representation:

```fsharp
type OrderRow = { totalCents: int64; currency: Currency }
```

This is permitted when the boundary converts it before domain use. Also permit temporary numeric intermediates that never leave a monetary calculation.
