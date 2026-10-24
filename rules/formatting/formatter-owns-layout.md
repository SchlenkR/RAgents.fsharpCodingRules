---
id: formatter-owns-layout
category: formatting
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Microsoft F# formatting guidelines, automatic code formatting: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting"
---

# Leave layout to the formatter

Write new code in the layout Fantomas produces with the repository's `.editorconfig`, and change the layout only of lines whose content you change. Hand-made layout drifts from the tool, and whitespace-only hunks hide the real change in a review.

## Check

1. Does the changeset contain hunks that only change indentation, line breaks, or bracket placement of code whose content stays the same?
2. Does new code deviate from Fantomas output, for example hand-aligned `=`, `->`, or comments, or lines wrapped by hand?
3. Was the formatter run on a whole file that was not formatted before, pulling unrelated lines into the diff?

## Report

The whitespace-only hunk or hand layout, corrected to the original lines or to Fantomas output for the changed lines only.

## Do not report

- Whitespace changes that Fantomas makes because nearby content changed.
- A separate commit that only applies the formatter and says so.

## Examples (illustrative, not exhaustive)

```fsharp
-let total = lines |> List.sumBy _.amount
+let total =
+    lines |> List.sumBy _.amount // finding: only re-wrapped

// correction: keep the line as it was
let total = lines |> List.sumBy _.amount

let id     = 1 // finding: aligned by hand
let amount = 20m

// correction
let id = 1
let amount = 20m
```

No finding: a line that Fantomas re-wrapped because its expression grew.
