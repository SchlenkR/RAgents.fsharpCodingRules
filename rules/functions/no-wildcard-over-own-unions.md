---
id: no-wildcard-over-own-unions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Spell out every case of your own unions

Write every case of a union defined in your code base in a match, grouping cases with the same result as or-patterns, instead of collecting them in a `| _ ->` branch.

## Look for

Look for matches on a union owned by the project where `_` or a catch-all binding accepts remaining cases:

```fsharp
match failure with
| ImportFailure.InvalidRow row -> showRowError row
| _ -> showErrorPage ()
```

A catch-all can also be hidden inside tuple/nested patterns; inspect the type of the matched component.

## Check the context

1. Resolve the union declaration and enumerate its cases. Establish that this project controls additions to it.
2. Identify the cases currently covered by the fallback, accounting for guards and earlier patterns.
3. Preserve payload bindings or use of the original value in the fallback. Check completeness after replacing it; guarded patterns alone may still leave cases uncovered.

## Report when

Report an unguarded catch-all that hides owned domain cases. Explicit coverage allows the compiler to identify the match when a case is added. Name the cases the wildcard currently stands for; do not claim a new behavior is necessary merely because they share a result.

## Suggest

List the remaining cases as or-patterns:

```fsharp
match failure with
| ImportFailure.InvalidRow row -> showRowError row
| ImportFailure.FileMissing
| ImportFailure.FileLocked
| ImportFailure.Timeout -> showErrorPage ()
```

Keep the existing fallback behavior for those cases. If it needs payloads, use explicit case branches or compatible bindings rather than discarding information.

## Do not report

Leave fallbacks over an open set unchanged:

```fsharp
match statusCode with
| 200 -> Ok body
| code -> Error code
```

Numbers and strings cannot be enumerated as project-owned union cases. Also exclude externally owned unions and tuples of fixed boolean combinations where adding a domain case is impossible.
