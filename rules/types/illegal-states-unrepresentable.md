---
id: illegal-states-unrepresentable
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Scott Wlaschin, Making illegal states unrepresentable: https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/"
  - "Scott Wlaschin, Domain Modeling Made Functional, chapter 5"
---

# Make illegal states unrepresentable

Model a value whose fields are valid only in certain combinations as a union with one case per state, each case carrying only the data that exists in that state. Flags and options that must agree with each other admit combinations no code handles, and the compiler cannot point to them.

## Check

1. For every new or changed record: is there a flag or status next to optional fields that only make sense in one state?
2. Are there two options of which exactly one, or at most one, may be set?
3. Is there a `Map<string, _>` for a fixed, known set of keys, such as the fields of a form?
4. Look for runtime checks guarding such combinations: `if payment.isPaid && payment.paidAt.IsNone`.

## Report

The type, a forbidden combination it admits, and the type that excludes it.

## Do not report

- Optional fields that are really independent, such as two optional search filters.
- Read-only query projections that mirror a table.

## Examples (illustrative, not exhaustive)

```fsharp
type Payment =
    {
        isPaid: bool
        paidAt: DateTime option // finding: isPaid = true with paidAt = None is possible
    }

// correction
type Payment =
    | Unpaid
    | Paid of paidAt: DateTime
```

No finding: `type SearchFilter = { customer: string option; from: DateOnly option }`.
