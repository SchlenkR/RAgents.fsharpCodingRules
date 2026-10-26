---
id: active-patterns-for-repeated-distinctions
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#, part 1, what to do when patterns get big and complex: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
---

# Name a repeated distinction with an active pattern

Give a distinction that several matches repeat, such as which plans include an admin area, a name as a complete active pattern, instead of grouping the same cases or testing the same condition in every match.

## Look for

Look for several matches grouping the same union cases to answer the same domain question. Search other consumers of the union, rather than treating the first grouping as a violation:

```fsharp
let showNavigation plan =
    match plan with
    | Plan.Team _
    | Plan.Enterprise _ -> showAdminMenu ()
    | Plan.Free
    | Plan.Personal _ -> ()

let canManageMembers plan =
    match plan with
    | Plan.Team _
    | Plan.Enterprise _ -> true
    | Plan.Free
    | Plan.Personal _ -> false
```

## Check the context

1. Compare the meaning of the groupings, not just their case lists. Do both consumers ask whether the plan includes administration?
2. Check whether an existing projection or active pattern already names this distinction.
3. Check that classifying the value is pure and preserves exhaustive coverage when the union changes.

## Report when

Report repeated classification of the same domain distinction when a named complete active pattern would express that question at its consumers. Independent copies can drift when a plan changes. Include the repeated consumers as evidence; a grouping in one match is insufficient.

## Suggest

Name the distinction once, then use that name in the matches:

```fsharp
let (|WithAdmin|WithoutAdmin|) plan =
    match plan with
    | Plan.Team _
    | Plan.Enterprise _ -> WithAdmin
    | Plan.Free
    | Plan.Personal _ -> WithoutAdmin

let canManageMembers plan =
    match plan with
    | WithAdmin -> true
    | WithoutAdmin -> false
```

Update the other consumers of this same distinction. Keep unrelated classifications separate.

## Do not report

Leave a single direct classification unchanged:

```fsharp
match plan with
| Plan.Team _
| Plan.Enterprise _ -> showAdminMenu ()
| Plan.Free
| Plan.Personal _ -> ()
```

When this is the only consumer of the grouping, extraction adds a name without removing repeated domain logic. Also leave clear single-case tests and coincidentally identical groups with different meanings unchanged.
