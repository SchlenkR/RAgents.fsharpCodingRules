---
id: nest-single-use-bindings
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [top-level-helpers]
sources:
  - "FSharpLint FL0091 FavourNestedFunctions: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0091.md"
---

# Nest single-use bindings in their consumer

Declare a function or value that only one function uses inside that function, directly below its head, and move it to module level when a second consumer appears. The smallest scope tells the reader where a helper matters and keeps the module surface small.

## Check

1. For every new or moved module-level `let`, including `let private`: how many functions in the file or project use it?
2. For every nested helper: does it sit at the top of its consumer's body, not buried in the deepest subexpression or after the code that uses it?

## Report

The single-use module-level binding, corrected to a nested `let` in its consumer.

## Do not report

- Bindings used by two or more functions, by tests, or by other files.
- Public API of a library module.
- Values that must be computed once, such as a compiled `Regex` or a lookup table; nesting would rebuild them on every call.

## Examples (illustrative, not exhaustive)

```fsharp
let private normalize (key: string) = key.Trim().ToLowerInvariant() // finding: only lookup uses it

let lookup table key = table |> Map.tryFind (normalize key)

// correction
let lookup table key =
    let normalize (key: string) = key.Trim().ToLowerInvariant()

    table |> Map.tryFind (normalize key)
```

No finding: a `let private` helper that both `lookup` and `tryRemove` call.
