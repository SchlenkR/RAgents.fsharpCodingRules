---
id: functions-in-companion-module
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [members-for-own-data]
sources:
  - "Scott Wlaschin, Domain Modeling Made Functional, chapters 5 and 6"
---

# Keep types as data and functions in a companion module

Define records and unions as plain data and put their functions, including construction and projections, in a module of the same name directly after the type, such as `Invoice.total`. Data and behavior stay separate, every function is curried and pipeable, and the types read as a compact domain model.

## Check

1. For every new member on a record or union: is it more than an interface implementation or override that .NET requires?
2. Do the functions for a type live in a module named after it, placed after the type?

## Report

The member and its module function.

## Do not report

- Interface implementations and overrides required by .NET, such as `IComparable`.
- Static members that operators or serializers need.

## Examples (illustrative, not exhaustive)

```fsharp
type Invoice =
    {
        lines: InvoiceLine list
    }

    member this.Total = this.lines |> List.sumBy _.amount // finding: behavior on the data type

// correction
module Invoice =
    let total (invoice: Invoice) = invoice.lines |> List.sumBy _.amount
```

No finding: `interface IComparable with ...` on a type used as a sort key.
