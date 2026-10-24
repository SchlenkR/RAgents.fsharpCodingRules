---
id: annotate-only-where-needed
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [annotate-public-signatures]
---

# Annotate types only where inference needs it

Write a type annotation only where the compiler needs it, not for clarity or out of habit, and decide by removing it and building. An unneeded annotation goes stale after a rename, repeats what tooltips show anyway, and hides which types inference really determines.

## Check

1. For every new annotation on a `let` parameter or return type: does the body already fix the type through a call, a match on union cases, or a record label?
2. Remove all candidate annotations together, then build; annotated functions keep their callers typed, so one at a time each looks necessary.

## Report

The annotations the build does not need.

## Do not report

- Record fields and union case fields, which are declarations.
- Member parameters, which F# does not infer, and parameters whose only use is a member access.
- Annotations that keep a unit of measure.

## Examples (illustrative, not exhaustive)

```fsharp
let total (lines: InvoiceLine list) : decimal = // finding: inference knows both types
    lines |> List.sumBy _.amount

// correction
let total lines = lines |> List.sumBy _.amount
```

No finding: `let shout (text: string) = text.ToUpperInvariant()`, where the member access needs the type.
