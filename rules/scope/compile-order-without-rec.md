---
id: compile-order-without-rec
category: scope
check: semantic
files: ["**/*.{fs,fsx,fsi}", "**/*.fsproj"]
sources:
  - "Scott Wlaschin, Cyclic dependencies are evil: https://fsharpforfunandprofit.com/posts/cyclic-dependencies/"
---

# Resolve dependency cycles instead of using rec

Declare dependencies before their users and resolve avoidable cycles through structure rather than recursive modules or groups.

## Look for

Look for `module rec`, `namespace rec`, and new `type ... and ...` or `let rec ... and ...` groups. Inspect related file-order changes in the project file:

```fsharp
module rec Billing =
    let total (invoice: Invoice) = invoice.amounts |> List.sum

    type Invoice = { amounts: decimal list }
```

Here recursion makes a later type visible to an earlier function.

## Check the context

Trace dependencies between declarations and files. Can an acyclic order express them directly? If two modules refer to each other, identify shared data or an operation that can become an explicit parameter. Distinguish genuine recursive data and traversal from declarations grouped merely for convenience. A `.fsproj` reorder alone is legitimate when it restores dependency order; inspect the resulting graph before claiming a cycle.

## Report when

Report a recursive scope or group that hides a removable forward reference or architectural cycle. Name the dependency path and a concrete way to break it. Broad recursive scopes make otherwise independent declarations depend on each other and obscure which layer owns the shared concept.

## Suggest

Reorder declarations when the dependency is one-way:

```fsharp
module Billing =
    type Invoice = { amounts: decimal list }

    let total (invoice: Invoice) = invoice.amounts |> List.sum
```

For a real module cycle, extract shared data into an earlier file or pass the needed function explicitly. Update project compile order with the structural change instead of adding another recursive scope.

## Do not report

Keep genuinely mutually recursive data:

```fsharp
type Expression =
    | Number of int
    | Block of Statement list

and Statement =
    | Print of Expression
    | Assign of string * Expression
```

Expressions contain statements and statements contain expressions, so declaration reordering cannot remove the dependency. Mutually recursive functions traversing such data are also permitted.
