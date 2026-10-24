---
id: active-patterns-for-repeated-distinctions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#, part 1, what to do when patterns get big and complex: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
---

# Name a repeated distinction with an active pattern

Give a distinction that several matches repeat, such as which plans include an admin area, a name as a complete active pattern, instead of grouping the same cases or testing the same condition in every match. The rule then lives in one place and keeps the compiler's completeness check.

## Check

1. Do two or more matches group the same cases or test the same condition to answer the same question?
2. Would a named pattern make that question visible at each match?

## Report

The repeated grouping and the active pattern that names it.

## Do not report

- A grouping that occurs once.
- A distinction that is a single case test, where matching the case directly is clear.

## Examples (illustrative, not exhaustive)

```fsharp
match plan with // finding: the same grouping appears in three functions
| Plan.Team _
| Plan.Enterprise _ -> showAdminMenu ()
| Plan.Free
| Plan.Personal _ -> ()

// correction
let (|WithAdmin|WithoutAdmin|) plan =
    match plan with
    | Plan.Team _
    | Plan.Enterprise _ -> WithAdmin
    | Plan.Free
    | Plan.Personal _ -> WithoutAdmin

match plan with
| WithAdmin -> showAdminMenu ()
| WithoutAdmin -> ()
```

No finding: one match in one function that groups these cases.
