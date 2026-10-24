---
id: no-wildcard-over-own-unions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Spell out every case of your own unions

Write every case of a union defined in your code base in a match, grouping cases with the same result as or-patterns, instead of collecting them in a `| _ ->` branch. A new case then shows up as a compiler warning at every match instead of silently taking the wildcard branch.

## Check

1. For every new or changed match over a union from this code base: does `| _ ->` or a catch-all name such as `| other ->` stand for one or more cases?
2. Does the catch-all hide cases that need different handling?

## Report

The wildcard and the or-pattern that lists its cases.

## Do not report

- Matches over open sets: strings, numbers, or unions owned by an external library.
- Matches over tuples of booleans or other fixed combinations, where no case can be added.

## Examples (illustrative, not exhaustive)

```fsharp
match failure with
| ImportFailure.InvalidRow row -> showRowError row
| _ -> showErrorPage () // finding: three cases hidden here

// correction
match failure with
| ImportFailure.InvalidRow row -> showRowError row
| ImportFailure.FileMissing
| ImportFailure.FileLocked
| ImportFailure.Timeout -> showErrorPage ()
```

No finding: `match statusCode with | 200 -> Ok body | code -> Error code`.
