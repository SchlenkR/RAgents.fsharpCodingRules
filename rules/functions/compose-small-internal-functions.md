---
id: compose-small-internal-functions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [name-the-data]
---

# Compose short internal transformations directly

Define a short function that only its own module uses as a typed composition or partial application when its steps and input type are evident.

## Look for

Look for internal forwarding wrappers that only feed their argument through one or two named transformations:

```fsharp
let sortedDistinct (values: int list) = values |> List.distinct |> List.sort
```

Inspect references to establish whether callers outside the module depend on named parameters.

## Check the context

1. Confirm that the function is internal to its module and that its steps are short, named transformations.
2. Check whether partial application or composition would evaluate any argument or factory once at definition time instead of on each call.
3. Compare inferred types, value restriction, and useful debugging points after the proposed rewrite.

## Report when

Report a forwarding wrapper when a short typed composition expresses the same internal operation with unchanged evaluation and an evident input type. This is the selected preference over rule `name-the-data`; do not apply both positions to the same definition.

## Suggest

Name the function type and compose the transformations:

```fsharp
let sortedDistinct: int list -> int list = List.distinct >> List.sort
```

Use partial application for one named operation when its captured arguments can safely be evaluated once. Keep a wrapper when those arguments must be obtained afresh.

## Do not report

Leave a wrapper that creates a fresh dependency on each call:

```fsharp
let formatted value = formatWith (loadCurrentFormat ()) value
```

Changing this to `let formatted = formatWith (loadCurrentFormat ())` would change when settings are read. Also leave public functions with useful parameter names and compositions that obscure inference or debugging.
