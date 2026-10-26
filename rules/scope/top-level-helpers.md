---
id: top-level-helpers
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [nest-single-use-bindings]
---

# Keep helper functions flat at module level

Declare substantial named helper functions at module level directly above their consumer when they do not capture the consumer's local values.

## Look for

Look for a function defined inside another function whose body spans several lines or defines more helpers:

```fsharp
let summarize (orders: Order list) =
    let describe (order: Order) =
        let total = order.amounts |> List.sum
        $"{order.id}: {total}"

    orders |> List.map describe
```

`describe` appears independent of the enclosing function's inputs.

## Check the context

Inspect free variables: does the helper capture parameters, local values, or another nested helper? Check whether it can move without adding parameters or changing initialization and effect timing. Distinguish a function declaration from a local value and from a binding inside a computation or list expression. Inspect neighboring module declarations to choose a position after the helper's dependencies and immediately before its consumer.

## Report when

Report a substantial nested helper that has no required closure and can move unchanged to module level. State the evidence that it is independent. The selected preference keeps the consumer shallow and lets readers inspect the helper separately. Length alone is insufficient when moving would require threading several captured values through new parameters.

## Suggest

Lift the independent helper directly above its consumer:

```fsharp
let describe (order: Order) =
    let total = order.amounts |> List.sum
    $"{order.id}: {total}"

let summarize (orders: Order list) = orders |> List.map describe
```

Choose accessibility according to the selected access-modifier rule. Preserve the helper's argument order and behavior; this rule does not require a separate file. It is the alternative to rule `nest-single-use-bindings`.

## Do not report

Keep a closure whose local input gives it meaning:

```fsharp
let overdue today (orders: Order list) =
    let isLate (order: Order) =
        let isOpen = order.closedAt.IsNone
        isOpen && order.due < today

    orders |> List.filter isLate
```

`isLate` captures `today`; lifting it would add a parameter. Also permit local values, one-line helpers, and bindings inside computation expressions and list expressions.
