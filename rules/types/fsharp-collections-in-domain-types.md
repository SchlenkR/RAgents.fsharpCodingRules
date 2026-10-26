---
id: fsharp-collections-in-domain-types
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use F# collections in domain types and signatures

Use immutable F# collections in domain fields and signatures instead of exposing mutable collection objects.

## Look for

Look for `ResizeArray`, .NET `List<T>`, `Dictionary`, and `HashSet` in domain records, unions, parameters, and return types:

```fsharp
type Basket = { items: ResizeArray<Item> }

let addItem item basket = basket.items.Add item
```

The record is immutable, but every holder can mutate the same item collection.

## Check the context

Determine whether this is shared domain data, a local construction buffer, or an infrastructure boundary. Trace aliasing and mutations through callers. Choose list, map, or set according to ordering, key lookup, and uniqueness requirements, preserving comparers and duplicate handling where relevant. Distinguish an intentionally owned concurrent cache from a mutable collection leaked into a domain value.

## Report when

Report mutable collection contracts in domain data when holders can change shared contents and no boundary or owned-state requirement justifies them. Name the exposed mutation and the immutable contract that suits the operation. Read-only record fields do not make referenced mutable collections immutable.

## Suggest

Expose an immutable collection and return replacement domain values:

```fsharp
type Basket = { items: Item list }

let addItem item basket =
    { basket with items = basket.items @ [ item ] }
```

This preserves append order without mutating other holders. Adapt mutation-based callers deliberately; the new function returns an updated basket. Convert mutable input to an immutable snapshot at entry, and choose `Map` or `Set` only when their key and ordering semantics match.

## Do not report

Keep mutable buffers confined to producing an immutable result:

```fsharp
let values (read: unit -> int) count =
    let buffer = ResizeArray<int>()

    for _ in 1..count do
        buffer.Add(read ())

    buffer |> Seq.toList
```

No buffer escapes. Rule `confine-mutation` governs its scope. Also permit interop and infrastructure boundaries and an owned `ConcurrentDictionary` cache whose synchronization is part of its responsibility.
