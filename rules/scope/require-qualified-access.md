---
id: require-qualified-access
category: scope
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Require qualified access for generic names

Mark a union `[<RequireQualifiedAccess>]` when a case name alone does not say which type it belongs to, and a module when its function names are generic, instead of prefixing every name by hand. Generic names collide after `open`: the most recently opened case wins, and readers cannot tell the types apart.

## Check

1. For every new union: could a case name such as `Pending`, `Failed`, `Empty`, `Custom`, or `Other` plausibly belong to another type in the project?
2. Look for hand-made prefixes on cases (`OsPending`, `PayFailed`) or on functions (`orderLoad`, `orderSave`).
3. For every new module with short function names such as `load`, `run`, or `create`: would opening it make call sites ambiguous?

## Report

The union or module, the attribute, and the names without prefixes.

## Do not report

- Specific case names that are unique in the project, such as `CardDeclined`.
- Modules meant to be opened, such as operator or DSL modules marked `[<AutoOpen>]`.

## Examples (illustrative, not exhaustive)

```fsharp
type OrderStatus = // finding: prefixes instead of the attribute
    | OsPending
    | OsShipped

// correction
[<RequireQualifiedAccess>]
type OrderStatus =
    | Pending
    | Shipped
```

No finding: `type PaymentFailure = CardDeclined | InsufficientFunds` without the attribute.
