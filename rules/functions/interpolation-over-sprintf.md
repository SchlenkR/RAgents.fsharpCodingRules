---
id: interpolation-over-sprintf
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Build strings with interpolation

Build strings with interpolated strings such as `$"Order {order.id} shipped"` instead of `sprintf`, `failwithf`, `String.Format`, or chains of `+`. Interpolation puts each value where it appears in the text, and typed holes such as `%d{count}` keep the compiler's format check.

## Check

1. Look for `sprintf`, `failwithf`, `String.Format`, and `+` chains that join text and values.
2. Is a partially applied `sprintf` used as a formatter, which a lambda with interpolation states more plainly?

## Report

The call and its interpolated form.

## Do not report

- Message templates that a logging library requires, such as `"Order {OrderId} shipped"` passed with arguments.
- Interpolating function values; rule `never-interpolate-functions` covers that.

## Examples (illustrative, not exhaustive)

```fsharp
let message = sprintf "Order %d shipped to %s" order.id customer.name // finding

// correction
let message = $"Order {order.id} shipped to {customer.name}"
```

No finding: `logger.LogInformation("Order {OrderId} shipped", order.id)`, a structured logging template.
