---
id: interpolation-over-sprintf
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Build strings with interpolation

Build strings with interpolated strings such as `$"Order {order.id} shipped"` instead of `sprintf`, `failwithf`, `String.Format`, or chains of `+`.

## Look for

Look for `sprintf`, `failwithf`, `System.String.Format`, concatenations of values with text, or a partially applied formatting function:

```fsharp
let message = sprintf "Order %d shipped to %s" order.id customer.name
let labels = orders |> List.map (sprintf "Order %d")
```

## Check the context

1. Check whether the format is ordinary display text or a structured logging/message template consumed by another API.
2. Preserve format specifiers, escaping, culture, alignment, and evaluation order. A typed hole may be necessary to retain the original format.
3. Resolve interpolated expressions to values; rule `never-interpolate-functions` covers accidentally interpolated function values.

## Report when

Report ordinary text assembly when interpolation puts values next to their text without changing formatting behavior. Include the equivalent format. Do not simplify a numeric/date format or replace a culture-sensitive API call without preserving its contract.

## Suggest

Place the values in the string; use a lambda where the original formatter was partially applied:

```fsharp
let message = $"Order %d{order.id} shipped to %s{customer.name}"
let labels = orders |> List.map (fun orderId -> $"Order %d{orderId}")
```

Replace `failwithf` with `failwith` of the equivalent interpolated message while preserving the exception path.

## Do not report

Leave structured logging templates unchanged:

```fsharp
logger.LogInformation("Order {OrderId} shipped", order.id)
```

The logger needs the template and separate argument to retain the named property. Also exclude format strings supplied as data when interpolation cannot represent the same contract.
