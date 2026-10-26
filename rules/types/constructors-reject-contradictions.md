---
id: constructors-reject-contradictions
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Reject contradictory input in constructors

Make constructors and conversions into validated domain types reject input they cannot represent faithfully instead of silently guessing a valid state.

## Look for

Look for wildcard branches, fallback defaults, and direct wrapper construction in `TryCreate`, `ofRow`, or `fromDto` conversions. Include values read from storage:

```fsharp
let ofStored (plan: string) (renewsAt: System.DateTime option) =
    match plan, renewsAt with
    | "paid", Some date -> Paid date
    | _ -> Free
```

A paid plan without a renewal date, an unknown plan, and a valid free plan all become `Free`.

## Check the context

Read the destination type's invariants and the input contract. Enumerate valid combinations rather than assuming the wildcard means missing data. Trace database and DTO conversions through validating constructors; `Option.map Wrapper` can bypass the same checks applied to user input. Determine whether a fallback is an explicit domain default, a documented absence, or an attempt to conceal inconsistent data.

## Report when

Report a conversion that accepts a forbidden combination, bypasses validation, or replaces unknown input with a plausible but unjustified state. Name a concrete input and the false state it produces. Stored data can violate invariants too; silently repairing it makes later decisions rely on fabricated information.

## Suggest

Handle each valid combination explicitly and return an error for the rest:

```fsharp
let tryOfStored (plan: string) (renewsAt: System.DateTime option) =
    match plan, renewsAt with
    | "paid", Some date -> Ok(Paid date)
    | "free", None -> Ok Free
    | _ -> Error(InvalidPlan(plan, renewsAt))
```

Route wrapped primitives through their validating constructors. Update callers to handle the new failure instead of immediately replacing it with another default; the intended behavior change is that contradictory input is rejected.

## Do not report

Keep defaults the domain actually defines:

```fsharp
let discountRate coupon =
    match coupon with
    | Some rate -> rate
    | None -> 0m
```

Here absence explicitly means no discount, rather than a broken discount becoming valid. Also exclude test data and scripts, and optional parsing where an empty field legitimately means no value.
