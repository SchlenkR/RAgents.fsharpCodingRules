---
id: fsharp-collections-in-domain-types
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use F# collections in domain types and signatures

Give record fields, union cases, and function signatures of domain code immutable F# collections such as `list`, `Map`, and `Set` instead of `ResizeArray`, `Dictionary`, or `HashSet`. A mutable collection inside a shared value can change under every holder, and its type invites the change.

## Check

1. Look for `ResizeArray`, `List<T>`, `Dictionary`, and `HashSet` in record fields, union cases, and signatures of domain modules.
2. Is a mutable collection confined to the function that builds an immutable result (rule `confine-mutation`)?

## Report

The field or signature and its F# collection type.

## Do not report

- Mutable collections confined to the function that builds a result.
- Interop and infrastructure boundaries, and a `ConcurrentDictionary` for state shared between threads, such as a cache.

## Examples (illustrative, not exhaustive)

```fsharp
type Basket = { items: ResizeArray<Item> } // finding: every holder can change the items

// correction
type Basket = { items: Item list }
```

No finding: a cache module that keeps a `ConcurrentDictionary` behind its own functions.
