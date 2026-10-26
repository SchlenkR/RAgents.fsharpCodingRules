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

Define records and unions as plain data and put their functions, including construction and projections, in a module of the same name directly after the type.

## Look for

Look for ordinary behavior members on records or unions, including projections, constructors, and state transitions. Also look for type-specific functions scattered through unrelated modules:

```fsharp
type Invoice =
    {
        amounts: decimal list
    }

    member this.Total = this.amounts |> List.sum

let amountDue (invoice: Invoice) = invoice.Total
```

## Check the context

Determine whether a member implements a required .NET contract, an override, or a static operation required by operators or a serializer. Inspect callers and signature files before moving behavior. Identify the companion module and check that it can appear directly after the type without introducing dependency cycles.

## Report when

Report ordinary record or union behavior outside its companion module when this preference is selected. Show which member or displaced function belongs with the type's functions. Keeping data declarations compact and functions curried makes the API consistent; an ordinary member is a preference violation even when it computes correctly.

## Suggest

Keep the data declaration and place its functions immediately after it:

```fsharp
type Invoice = { amounts: decimal list }

module Invoice =
    let total (invoice: Invoice) = invoice.amounts |> List.sum

let amountDue invoice = Invoice.total invoice
```

Move construction and transitions similarly, preserving validation and updating callers and signatures. This rule is the alternative to rule `members-for-own-data`.

## Do not report

Keep members needed to implement a .NET contract:

```fsharp
type TextSource(fileName: string) =
    let reader = System.IO.File.OpenText fileName

    member _.Read() = reader.ReadToEnd()

    interface System.IDisposable with
        member _.Dispose() = reader.Dispose()
```

`Dispose` must implement the interface. This class also owns a resource rather than being a data-only record or union. Retain required overrides and static members needed by operators or serializers; their contract cannot be met by an ordinary module function.
