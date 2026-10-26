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


## Look for

Look for diff hunks changing only layout of otherwise untouched code, and hand-aligned new bindings that a formatter will immediately rewrite.

```diff
-let total = lines |> List.sumBy _.amount
+let total =
+    lines |> List.sumBy _.amount
```

## Check the context

1. Compare tokens before and after the hunk. Is the change semantic, required by nearby edited code, or independent reformatting?
2. Inspect the configured Fantomas version and `.editorconfig`; use its output to establish the expected layout.
3. Check whether this is an explicitly separate formatting change. Whole-file formatting can pull old layout into a functional diff even when the author did not edit it manually.

## Report when

Report unrelated layout churn in a functional changeset, explaining which untouched expression was reflowed. Delegate exact indentation and bracket choices to Fantomas. This semantic check concerns review scope and unnecessary diff noise; it does not ask a model to reproduce the formatter's layout rules.

## Suggest

Restore unrelated lines and let Fantomas format new or substantively changed code using the repository configuration:

```fsharp
let total = lines |> List.sumBy _.amount
```

If the file needs a broad formatting cleanup, suggest a separately identified formatting change. Do not undo line breaks or indentation that Fantomas requires because the surrounding expression changed.

## Do not report

Keep formatter output caused by an actual expression change:

```diff
-let total = lines |> List.sumBy _.amount
+let total =
+    lines
+    |> List.filter _.isBillable
+    |> List.sumBy _.amount
```

Here the pipeline gained a filtering step, so reflow belongs to the change. A clearly identified change devoted solely to applying the formatter is also permitted; its scope is already explicit.
