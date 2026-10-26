---
id: describe-instead-of-tostring
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name text projections instead of overriding ToString

Give a type a named member such as `Description` for its display text instead of overriding `ToString`. Interpolation, concatenation, and logging call `ToString` implicitly, so nobody can find its callers, and changing it breaks them silently.


## Look for

Look for project-defined `ToString` overrides and call sites interpolating or otherwise implicitly formatting those values as display text.

```fsharp
type Order =
    {
        id: int
    }

    override this.ToString() = $"Order #{this.id}"

let confirmation (order: Order) = $"Shipped: {order}"
```

## Check the context

1. Inspect the override's purpose: user-facing description, an explicit .NET formatting contract, or debugging only.
2. Find interpolation, concatenation, logging, and explicit `ToString` calls relying on that representation.
3. Check public API and framework requirements before removing the override; callers outside the repository may depend on its behavior.

## Report when

Report a display projection implemented as an implicit `ToString` override without a required formatting contract. Include affected callers. This preference makes display use explicit and discoverable, so changing the projection does not silently change unrelated interpolation and logs.

## Suggest

Expose the display text through a named projection and update its consumers:

```fsharp
type Order =
    {
        id: int
    }

    member this.Description = $"Order #{this.id}"

let confirmation (order: Order) = $"Shipped: {order.Description}"
```

Choose a more specific projection name if several text formats exist. Preserve published compatibility where necessary instead of removing an override without inspecting consumers.

## Do not report

Keep a view that already asks explicitly for the projection:

```fsharp
let confirmation (order: Order) = $"Shipped: {order.Description}"
```

The call site names the representation it needs. Types implementing an established .NET formatting contract such as `IFormattable`, and debugging attributes such as `StructuredFormatDisplay` that leave `ToString` unchanged, are permitted.
