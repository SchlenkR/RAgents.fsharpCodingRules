---
id: units-of-measure
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Attach units of measure to quantities that meet other units

Use units of measure when numeric quantities of different units or scales meet in the same code.

## Look for

Look for unit-bearing names or comments in code handling another unit of the same dimension, and bare conversion factors such as `1000.0` or `100m`:

```fsharp
let totalWeight (gramsPerItem: float) (packagingKg: float) count =
    gramsPerItem * float count / 1000.0 + packagingKg
```

Both weights are plain floats, so a missing or inverted conversion remains type-correct.

## Check the context

Trace units from input through calculations and outputs. Verify the conversion factor and its direction rather than inferring them only from a variable name. Check whether an existing type such as `TimeSpan` or `Money` already expresses the unit. Establish that different units or scales actually meet; introducing measures for an isolated number adds no useful distinction.

## Report when

Report bare numeric quantities whose interacting units are only a naming convention and can be accidentally mixed. Identify the actual unit relationship and conversion. The aim is compiler-checked dimensional consistency, not annotating every arithmetic variable or replacing types that already supply the contract.

## Suggest

Measure the inputs and give the conversion factor its dimensional type:

```fsharp
[<Measure>]
type g

[<Measure>]
type kg

let gramsPerKilogram = 1000.0<g / kg>

let totalWeight (perItem: float<g>) (packaging: float<kg>) count =
    perItem * float count / gramsPerKilogram + packaging
```

Propagate measures through the relevant domain signatures and attach or remove them at external boundaries. Keep the conversion explicit; measures are erased at runtime and do not automatically convert values.

## Do not report

Leave a calculation using one unit consistently:

```fsharp
let totalMetres (lengths: float list) = lengths |> List.sum
```

If all callers and results use metres and no scale conversion occurs, this trigger is absent. Also retain `TimeSpan`, `DateTime`, and money types that already carry the needed meaning instead of layering redundant measures onto them.
