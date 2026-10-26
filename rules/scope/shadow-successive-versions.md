---
id: shadow-successive-versions
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Shadow successive versions of a value

Rebind successive replacements under the same name when later code needs only the current version.

## Look for

Look for numbered or prefixed versions such as `order1`, `updatedCart`, and `finalState`, and mutable bindings reassigned in a straight-line sequence:

```fsharp
let amountDue discount taxRate net =
    let amount1 = net - discount
    let amount2 = amount1 * (1m + taxRate)
    amount2
```

Each binding here is another version of the same amount.

## Check the context

Read all later uses of each version. Is an earlier value still needed for comparison, logging, rollback, or another branch? Check whether a step changes the value's meaning or type, such as parsing text into a number. For mutation, distinguish a linear replacement chain from a loop or a closure that intentionally shares mutable state. Shadowing must not turn captured mutable state into a snapshot.

## Report when

Report version names or sequential reassignments when each old value becomes obsolete and all later uses should refer to the latest version. Explain that relationship rather than flagging a name in isolation. Shadowing removes access to the stale binding without introducing reassignment.

## Suggest

Give each replacement the same immutable name:

```fsharp
let amountDue discount taxRate net =
    let amount = net - discount
    let amount = amount * (1m + taxRate)
    amount
```

Use repeated `let!` in a computation expression when each replacement is bound from an effectful or fallible step. Preserve effect order and ensure earlier values are not still needed.

## Do not report

Keep distinct versions when the operation compares them:

```fsharp
let change discount net =
    let before = net
    let after = net - discount
    before, after
```

Both values are part of the result, so shadowing would hide required information. Also use different names for values with different meanings, such as raw text and a parsed count, even when one is derived from the other.
