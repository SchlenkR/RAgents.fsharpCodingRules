---
id: shadow-successive-versions
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Shadow successive versions of a value

Rebind a value under the same name when each step replaces the previous version, instead of numbering the names or reassigning a mutable variable. Shadowing makes the outdated version unreachable, so no later line can use it by mistake.

## Check

1. Look for numbered or prefixed versions of one value: `order1` and `order2`, `newCart`, `updatedState`, `tmp`.
2. Look for a `let mutable` that is reassigned in sequence, outside a loop.

## Report

The chain, corrected to repeated `let` or `let!` bindings of the same name.

## Do not report

- Versions that are both used later, such as `before` and `after` in a comparison.
- Steps that change what a value means, such as text parsed into a number; they need different names.

## Examples (illustrative, not exhaustive)

```fsharp
taskResult {
    let! cart = loadCart cartId
    let! updatedCart = cart |> Cart.tryAddItem item // finding: cart stays usable below
    let! finalCart = updatedCart |> Cart.tryApplyCoupon coupon
    return finalCart
}

// correction
taskResult {
    let! cart = loadCart cartId
    let! cart = cart |> Cart.tryAddItem item
    let! cart = cart |> Cart.tryApplyCoupon coupon
    return cart
}
```

No finding: `let previous = state` and `let next = step state`, both compared in a test.
