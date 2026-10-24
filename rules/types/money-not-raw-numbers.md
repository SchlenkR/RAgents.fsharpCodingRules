---
id: money-not-raw-numbers
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Represent money as a type, not as raw numbers

Carry amounts of money as a `Money` value with a decimal amount and a currency instead of raw `int` cents or a bare `decimal`, and convert to cents only at the storage boundary, checking the range of the column and the domain's limits. Raw numbers mix up cents and units, lose the currency, and break when a large amount no longer fits a narrow column.

## Check

1. Look for `int` or `int64` fields named like amounts (`priceCents`, `totalPence`) and bare `decimal` amounts that travel between modules.
2. Where cents are produced for storage: is the result checked against the column's range and against domain limits such as a price above zero?

## Report

The raw amount and its `Money` form, or the missing range check.

## Do not report

- The storage row type, which holds cents by design and converts on entry.
- Amounts that never leave one calculation.

## Examples (illustrative, not exhaustive)

```fsharp
type Order = { totalCents: int } // finding: raw cents without currency

// correction
type Currency =
    | EUR
    | GBP

type Money = { amount: decimal; currency: Currency }

type Order = { total: Money }
```

No finding: `type OrderRow = { totalCents: int64 }`, the database row converted on entry.
