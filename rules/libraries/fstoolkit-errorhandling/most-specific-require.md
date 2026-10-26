---
id: most-specific-require
category: libraries/fstoolkit-errorhandling
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["FsToolkit.ErrorHandling"]
sources:
  - "FsToolkit.ErrorHandling, Result functions: https://demystifyfp.gitbook.io/fstoolkit-errorhandling/"
---

# Use the most specific require function

Express a check with the most specific `require` function of FsToolkit, such as `Result.requireEqual` for a comparison or `Result.requireNotEmpty` for a list, instead of `Result.requireTrue` over a hand-written condition.

## Look for

Look for generic boolean requirements wrapping comparisons, emptiness tests, or presence tests when a specific FsToolkit requirement expresses the same check:

```fsharp
result {
    do! List.isEmpty order.lines |> Result.requireFalse NoLines
    return order
}
```

Also inspect `requireSome` applied to an option manufactured solely to encode an unrelated predicate.

## Check the context

1. Resolve the predicate and the project's available `require` functions. Check argument order and the success value each helper returns.
2. Preserve the exact condition, error value, and evaluation timing. `requireSome` extracts a payload; replacing a boolean presence check may require binding or ignoring that payload explicitly.
3. Check that a specific helper does not enumerate an effectful source differently or change eager/lazy error construction.

## Report when

Report a hand-written generic requirement when an available specific helper expresses exactly the same condition more clearly. Name that condition and helper. Report manufactured options only when presence is not the actual domain question; do not force a superficially similar helper onto a different predicate.

## Suggest

Use the non-empty requirement directly:

```fsharp
result {
    do! order.lines |> Result.requireNotEmpty NoLines
    return order
}
```

For equality, choose `Result.requireEqual` or its piped `requireEqualTo` form with the correct argument order. For an actual optional payload, bind `Result.requireSome` instead of checking presence and extracting separately.

## Do not report

Leave a compound/range predicate without an equivalent specific helper:

```fsharp
result {
    do! (order.total > 0m && order.total <= limit) |> Result.requireTrue InvalidTotal
    return order
}
```

The generic requirement states this actual condition. Also exclude rewrites that would change returned payloads, error values, or evaluation behavior.
