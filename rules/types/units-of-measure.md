---
id: units-of-measure
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Attach units of measure to quantities that meet other units

Declare units of measure for numeric quantities when values of different units or scales meet in the same code, such as grams and kilograms or cents and euros. The compiler then rejects adding them, and every conversion becomes a named factor at no runtime cost.

## Check

1. Look for names or comments that carry a unit (`weightInGrams`, `timeoutMs`, `// in cents`) in code that also handles another unit of the same dimension.
2. Look for bare conversion factors such as `* 1000.0` or `/ 100m` applied to such values.

## Report

The quantity, its unit of measure, and the named conversion factor.

## Do not report

- A single unit used throughout a module without conversions.
- `TimeSpan`, `DateTime`, and money types that already carry their unit.

## Examples (illustrative, not exhaustive)

```fsharp
let totalWeight (gramsPerItem: float) (packagingKg: float) count =
    gramsPerItem * float count / 1000.0 + packagingKg // finding: bare factor between units

// correction
[<Measure>]
type g

[<Measure>]
type kg

let gramsPerKilogram = 1000.0<g / kg>

let totalWeight (perItem: float<g>) (packaging: float<kg>) count =
    perItem * float count / gramsPerKilogram + packaging
```

No finding: a module that computes only in metres.
