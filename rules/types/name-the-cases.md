---
id: name-the-cases
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name the cases instead of using bool or Choice

Represent domain alternatives with named union cases, and pass an existing domain distinction instead of reducing it to a boolean flag.

## Look for

Look for boolean parameters selecting behavior, `Choice` in signatures, and `bool * 'a` results representing found/not found. Inspect bare-flag calls and booleans derived from an existing union:

```fsharp
let export (includeArchived: bool) (orders: Order list) =
    orders |> List.filter (fun order -> includeArchived || not order.isArchived)

let allOrders orders = export true orders
```

`true` does not name the selected behavior at the call site.

## Check the context

Determine whether the boolean is a domain alternative or a genuine yes/no fact. Trace callers to see whether an existing union already carries the needed distinction. For `Choice`, identify the meaning of each branch. For paired found/value results, inspect whether the value is meaningless when the flag is false. Preserve .NET boundary contracts rather than changing framework APIs.

## Report when

Report behavior switches or unnamed domain alternatives whose meaning requires reading the implementation. Also report reducing a meaningful union to a flag solely to pass it onward. Identify the actual alternatives and the loss of information; an ordinary predicate returning bool is not a violation.

## Suggest

Name the behavior and make callers select it explicitly:

```fsharp
[<RequireQualifiedAccess>]
type Archived =
    | Include
    | Exclude

let export archived (orders: Order list) =
    orders
    |> List.filter (fun order ->
        match archived with
        | Archived.Include -> true
        | Archived.Exclude -> not order.isArchived)

let allOrders orders = export Archived.Include orders
```

Pass the existing plan or mode union when one already expresses the distinction. Replace `Choice` with a domain union and a found/value pair with `option`, updating callers to match the named outcomes.

## Do not report

Keep predicates and factual boolean data:

```fsharp
type Account = { isActive: bool }

let isEnabled account = account.isActive
```

This records and answers a yes/no fact; it does not select an unnamed strategy. Also permit .NET `TryParse` and `TryGetValue` results at the interop boundary.
