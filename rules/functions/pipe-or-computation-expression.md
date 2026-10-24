---
id: pipe-or-computation-expression
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Pipe straight chains, bind named steps in a computation expression

Write a straight chain of `Option`, `Result`, or `Task` steps that each take the previous result as a pipeline, and use a computation expression when a step needs an earlier result by name or the pipeline would nest lambdas. A pipeline hides nothing in a straight chain, and a computation expression names values instead of nesting closures.

## Check

1. Look for nested `Option.bind (fun a -> ...)` or `Result.bind` lambdas that only keep earlier results in scope.
2. Look for a computation expression that only maps or defaults one value.
3. Inside a computation expression: does every fallible step get its own `let!`, not a `bind` chain behind one `let!`?

## Report

The nested lambdas or the trivial expression, and the other form.

## Do not report

- A `match` whose branches do different real work.
- Projects without a builder for the type; `option` and `result` come from FsToolkit.ErrorHandling.

## Examples (illustrative, not exhaustive)

```fsharp
let shippingLabel customerId =
    tryFindCustomer customerId
    |> Option.bind (fun customer -> // finding: nested to keep customer in scope
        tryFindAddress customer.addressId
        |> Option.map (fun address -> $"{customer.name}, {address.city}"))

// correction
let shippingLabel customerId =
    option {
        let! customer = tryFindCustomer customerId
        let! address = tryFindAddress customer.addressId
        return $"{customer.name}, {address.city}"
    }
```

No finding: `tryFindCustomer id |> Option.map _.name |> Option.defaultValue "unknown"`.
