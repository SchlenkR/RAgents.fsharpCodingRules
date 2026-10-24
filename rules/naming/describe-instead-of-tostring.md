---
id: describe-instead-of-tostring
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name text projections instead of overriding ToString

Give a type a named member such as `Description` for its display text instead of overriding `ToString`. Interpolation, concatenation, and logging call `ToString` implicitly, so nobody can find its callers, and changing it breaks them silently.

## Check

1. Look for `override this.ToString()` on records, unions, and classes.
2. Look for interpolation or concatenation that relies on such an override, such as `$"Shipped: {order}"`.

## Report

The override, the named member that replaces it, and the callers that must use it.

## Do not report

- Types that implement a .NET formatting contract such as `IFormattable`.
- Debugging attributes such as `[<StructuredFormatDisplay>]`, which leave `ToString` alone.

## Examples (illustrative, not exhaustive)

```fsharp
type Order =
    {
        id: int
    }

    override this.ToString() = $"Order #{this.id}" // finding: callers depend on it invisibly

// correction
type Order =
    {
        id: int
    }

    member this.Description = $"Order #{this.id}"
```

No finding: a view that calls `order.Description` explicitly.
