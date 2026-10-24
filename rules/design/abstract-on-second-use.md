---
id: abstract-on-second-use
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Generalize only for a second real use

Generalize domain code for reuse, with type parameters, interfaces, switches, or shared helpers, only when a second real use exists, and let small incidental blocks repeat until then. Machinery for hypothetical callers makes the one real case harder to read and guesses a future that rarely arrives as expected.

## Check

1. Does a new abstraction serve consumers that do not exist yet, such as an explicit type parameter used with one type or an interface with one implementation?
2. Does a new optional parameter, setting, or union case serve a requirement nobody implements?
3. Does a shared helper parameterize incidental differences between call sites instead of expressing one domain operation?

## Report

The abstraction and its concrete form.

## Do not report

- Generic types the compiler inferred.
- Domain modeling, and helpers extracted to isolate a decision or an effect (rule `io-at-the-edges`).
- Library APIs meant for unknown callers.

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
