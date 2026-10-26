---
id: no-access-modifiers
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [hide-implementation-details]
---

# Leave access modifiers out

Write functions, values, types, and modules without access modifiers, retaining private cases on validated single-case unions.

## Look for

Look for `private` and `internal` on bindings, modules, types, members, and constructors:

```fsharp
module Keys =
    let private normalize (key: string) = key.Trim().ToLowerInvariant()

    let lookup table key = table |> Map.tryFind (normalize key)
```

Distinguish a declaration modifier from `private` protecting a union case.

## Check the context

Determine whether this is application code following the selected preference or a published library with a controlled public contract. Check whether a modifier protects validated construction, including a private single-case union case under rule `wrap-constrained-primitives`. Inspect signature files and API commitments before proposing a change to library visibility.

## Report when

Report ordinary declaration modifiers in code where this preference applies and neither exception holds. Name the modifier and the declaration it unnecessarily restricts. This is an explicit accessibility preference: module structure and file order organize the code, while direct access remains available to tests and scripts. It does not claim that restricted visibility causes a runtime defect.

## Suggest

Remove the declaration modifier without changing the operation:

```fsharp
module Keys =
    let normalize (key: string) = key.Trim().ToLowerInvariant()

    let lookup table key = table |> Map.tryFind (normalize key)
```

Keep validated construction protected. Do not combine the visibility change with unrelated helper movement; placement follows the selected scope rule. This rule is the alternative to rule `hide-implementation-details`.

## Do not report

Keep a validated union's case private:

```fsharp
type PositiveCount = private PositiveCount of int

module PositiveCount =
    let tryCreate value =
        if value > 0 then
            Ok(PositiveCount value)
        else
            Error "count must be positive"
```

The restriction prevents callers from bypassing the invariant. Also preserve modifiers required to maintain the intentionally controlled surface of a published library.
