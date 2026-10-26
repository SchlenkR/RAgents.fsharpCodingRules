---
id: require-qualified-access
category: scope
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Require qualified access for generic names

Mark unions and modules with generic names RequireQualifiedAccess instead of relying on unqualified names or manual prefixes.

## Look for

Look for generic union cases such as `Pending`, `Failed`, or `Empty`, manually prefixed cases, and module functions named `load`, `run`, or `create`:

```fsharp
type OrderStatus =
    | OsPending
    | OsShipped

let initialStatus = OsPending
```

The prefix is a candidate for qualification supplied by the type itself.

## Check the context

Inspect nearby types and open declarations. Could a case plausibly belong to another project type? Would opening a module make short function names ambiguous? Determine whether a prefix merely repeats its container or carries actual domain meaning. Check whether the module deliberately exports operators or a DSL through `AutoOpen`. Inspect callers, serialized case names, and public signatures before renaming.

## Report when

Report a generic name requiring its container for meaning when unqualified use is allowed, or manual prefixes that only compensate for missing qualification. Explain the ambiguity or repeated container name. Qualification makes the type or module visible at every call without inventing parallel naming conventions; a present-day compiler collision is not required.

## Suggest

Add the attribute and update uses to qualified, unprefixed names:

```fsharp
[<RequireQualifiedAccess>]
type OrderStatus =
    | Pending
    | Shipped

let initialStatus = OrderStatus.Pending
```

For a module, apply the same attribute and call operations as `Order.load`, removing redundant names such as `orderLoad`. Preserve meaningful prefixes. Check wire formats and compatibility before renaming published union cases.

## Do not report

Leave specific case names that identify the outcome themselves:

```fsharp
type PaymentFailure =
    | CardDeclined
    | InsufficientFunds
```

When these names are distinctive in the project, no container is needed to understand them. Modules deliberately meant to be opened, including operator and DSL modules marked `AutoOpen`, are also permitted.
