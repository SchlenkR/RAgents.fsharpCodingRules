---
id: abstract-on-second-use
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Abstract only for a second real use

Introduce a generic type parameter, an interface, a configuration switch, or a shared helper only when a second real use exists, not in anticipation, and let small blocks repeat until then. Speculative abstraction makes the one real case harder to read and guesses a future that rarely arrives as expected.

## Check

1. For every new generic parameter: is the function or type used with at least two different types?
2. For every new interface, union case, optional parameter, or setting: does code use each alternative?
3. For every extracted helper: is real logic shared, or only two similar lines?

## Report

The abstraction and its concrete form.

## Do not report

- Library APIs meant for unknown callers.
- Shared domain rules; this rule allows repeating small blocks, not repeating logic.

## Examples (illustrative, not exhaustive)

```fsharp
type Store<'entity, 'key> = // finding: only Store<Customer, CustomerId> exists
    {
        load: 'key -> Task<'entity option>
        save: 'entity -> Task<unit>
        delete: 'key -> Task<unit>
    }

// correction
type CustomerStore =
    {
        load: CustomerId -> Task<Customer option>
        save: Customer -> Task<unit>
        delete: CustomerId -> Task<unit>
    }
```

No finding: a generic `retry` helper used for HTTP calls and for database calls.
