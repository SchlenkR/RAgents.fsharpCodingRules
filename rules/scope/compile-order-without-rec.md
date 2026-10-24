---
id: compile-order-without-rec
category: scope
check: semantic
files: ["**/*.{fs,fsx,fsi}", "**/*.fsproj"]
sources:
  - "Scott Wlaschin, Cyclic dependencies are evil: https://fsharpforfunandprofit.com/posts/cyclic-dependencies/"
---

# Resolve dependency cycles instead of using rec

Declare types and functions before their users, in file order and within a file, and resolve a dependency cycle by restructuring rather than with `module rec`, `namespace rec`, or `and` between unrelated types. F# guarantees acyclic dependencies through compile order; recursive scopes give that guarantee away and let the cycles grow.

## Check

1. Look for `module rec` and `namespace rec`.
2. For every new `type ... and ...` or `let rec ... and ...` group: are the members really mutually recursive, or only declared in the wrong order?
3. Does a file move in the `.fsproj` only to make a later file visible to an earlier one?

## Report

The recursive scope and the restructuring: reorder, extract the shared type into an earlier file, or pass a function as a parameter.

## Do not report

- Genuinely recursive data, such as a syntax tree whose expressions contain statements and vice versa.
- Mutually recursive functions over such data.

## Examples (illustrative, not exhaustive)

```fsharp
module rec Billing = // finding: rec only hides that Invoice is declared after its user
    let total (invoice: Invoice) = invoice.lines |> List.sumBy _.amount

    type Invoice = { lines: InvoiceLine list }

// correction
module Billing =
    type Invoice = { lines: InvoiceLine list }

    let total (invoice: Invoice) = invoice.lines |> List.sumBy _.amount
```

No finding: `type Expr = ... | Block of Statement list and Statement = ... | Eval of Expr`.
