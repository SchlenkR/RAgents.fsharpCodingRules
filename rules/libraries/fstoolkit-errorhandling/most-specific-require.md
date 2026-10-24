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

Express a check with the most specific `require` function of FsToolkit, such as `Result.requireEqual` for a comparison or `Result.requireNotEmpty` for a list, instead of `Result.requireTrue` over a hand-written condition. The specific function names the condition and keeps the compared values visible.

## Check

1. Look for `requireTrue` and `requireFalse` over comparisons (`a = b`), emptiness tests (`List.isEmpty xs`), or option tests (`x.IsSome`).
2. Is `requireSome` used only where presence is the question, not to smuggle in another condition?

## Report

The check and its specific `require` function.

## Do not report

- Conditions that no specific function expresses, such as a range check.

## Examples (illustrative, not exhaustive)

```fsharp
result {
    do! List.isEmpty order.lines |> Result.requireFalse NoLines // finding
    return order
}

// correction
result {
    do! order.lines |> Result.requireNotEmpty NoLines
    return order
}
```

No finding: `(order.total > 0m) |> Result.requireTrue TotalNotPositive`.
