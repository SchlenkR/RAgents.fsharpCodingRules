---
id: empty-text-is-missing
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Treat empty text as missing where it enters

Convert text that may be null, empty, or whitespace into an `option` where it enters, so that all three mean missing in the same way everywhere. Databases and forms often hold empty strings and nulls side by side, and a check that knows only one of them treats the other as a real value.

## Check

1. Where text enters from a database, a form, a file, or an API: does `Option.ofObj` or a null check let `""` through as `Some ""`?
2. Is the same field tested for null in one place and for emptiness in another?

## Report

The conversion and its normalized form, in F# or with `NULLIF(column, '')` in the query.

## Do not report

- Text where empty and missing mean different things in the domain.

## Examples (illustrative, not exhaustive)

```fsharp
let middleName = Option.ofObj row.MiddleName // finding: "" becomes Some ""

// correction
let middleName =
    if String.IsNullOrWhiteSpace row.MiddleName then
        None
    else
        Some row.MiddleName
```

No finding: `Option.ofObj row.Comment` for a column whose constraint forbids empty strings.
