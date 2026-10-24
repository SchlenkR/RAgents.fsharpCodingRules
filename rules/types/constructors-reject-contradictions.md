---
id: constructors-reject-contradictions
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Reject contradictory input in constructors

Make every constructor of a validated type return an error for input it cannot represent faithfully, also for values loaded from a database, instead of guessing a default. A guessed default turns broken data into a plausible state, and every later decision builds on it.

## Check

1. For every constructor or conversion into a domain type (`TryCreate`, `ofRow`, `fromDto`): does a wildcard branch map unexpected combinations to some valid case?
2. Look for `Option.map Wrapper` or a direct `Wrapper value` that lifts a loaded value into a validated type without its check.
3. Does a missing or unknown part of the input silently become a default?

## Report

The tolerant branch and the error case it should return.

## Do not report

- Defaults the domain itself defines, such as no discount when no coupon is given.
- Test data and scripts.

## Examples (illustrative, not exhaustive)

```fsharp
let ofStored (plan: string) (renewsAt: DateTime option) =
    match plan, renewsAt with
    | "paid", Some date -> Paid date
    | _ -> Free // finding: a paid plan without renewal date silently becomes Free

// correction
let tryOfStored (plan: string) (renewsAt: DateTime option) =
    match plan, renewsAt with
    | "paid", Some date -> Ok(Paid date)
    | "free", None -> Ok Free
    | _ -> Error(InvalidPlan(plan, renewsAt))
```

No finding: `Coupon.tryParse` returning `None` for an empty field, where no coupon is a valid choice.
