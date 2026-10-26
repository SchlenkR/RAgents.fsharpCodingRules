---
id: pipe-or-computation-expression
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Pipe straight chains, bind named steps in a computation expression

Write a straight chain of `Option`, `Result`, or `Task` steps that each take the previous result as a pipeline, and use a computation expression when a step needs an earlier result by name or the pipeline would nest lambdas.

## Look for

Look for nested `Option.bind`, `Result.bind`, or project-provided task combinators whose inner callbacks retain earlier results. Also look for a computation expression around one simple map/default, or a bind chain hidden behind one `let!`:

```fsharp
let shippingLabel customerId =
    tryFindCustomer customerId
    |> Option.bind (fun customer ->
        tryFindAddress customer.addressId
        |> Option.map (fun address -> $"{customer.name}, {address.city}"))
```

## Check the context

1. Identify which earlier values the inner steps need; here the final text retains `customer.name`.
2. Verify that an appropriate builder already exists. The `option` example requires FsToolkit.ErrorHandling in scope; resolve task combinators rather than assuming their semantics.
3. Preserve short-circuiting, effects, evaluation order, and task cancellation/start behavior.
4. For a trivial expression, check whether ordinary map/default operations say the whole operation without losing branch behavior.

## Report when

Report nested continuations retained solely to keep earlier values available, or hidden fallible chains that named CE bindings would expose. In the other direction, report a CE that merely maps/defaults one value when a straight pipeline expresses the same behavior. State the specific simplification; the presence of `bind` alone is insufficient.

## Suggest

Name the intermediate results in the existing builder:

```fsharp
let shippingLabel customerId =
    option {
        let! customer = tryFindCustomer customerId
        let! address = tryFindAddress customer.addressId
        return $"{customer.name}, {address.city}"
    }
```

Within a CE, expose the individual fallible steps with `let!`. For a one-value projection, use a pipeline such as `tryFindCustomer id |> Option.map _.name`. Keep effectful steps sequential unless concurrency is independently justified.

## Do not report

Leave a straight pipeline using only each immediate input:

```fsharp
let customerCity customerId =
    tryFindCustomer customerId
    |> Option.bind (fun customer -> tryFindAddress customer.addressId)
    |> Option.map _.city
```

No nested callback retains an earlier result. Also leave matches with distinct branch work and code without a compatible builder; do not add a dependency solely for this preference.
