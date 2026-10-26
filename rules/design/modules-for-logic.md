---
id: modules-for-logic
category: design
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [classes-for-configured-logic]
sources:
  - "Microsoft F# coding conventions, use classes to contain values that have side effects: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
  - "Microsoft F# component design guidelines, use classes to encapsulate mutable state: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
  - "Isaac Abraham, Get Programming with F#, lesson 12"
---

# Use modules for logic and classes only for state with a lifetime

Write logic as functions over records and unions, and reserve classes for state, resource ownership, lifetimes, or required framework contracts.

## Look for

Look for stateless service classes that capture settings or dependencies, classes used only around a local calculation, and classes containing only data. Inspect construction and consumption:

```fsharp
type PriceCalculator(taxRate: decimal) =
    member _.Gross(net: decimal) = net * (1m + taxRate)

let total taxRate (amounts: decimal list) =
    let calculator = PriceCalculator taxRate
    amounts |> List.sumBy calculator.Gross

type PriceQuote(net: decimal, taxRate: decimal) =
    member _.Net = net
    member _.TaxRate = taxRate
```

## Check the context

Inspect constructors, fields, members, base types, interfaces, and references across the project, including tests and signature files. Follow instances: are they returned, stored, or passed to required contracts? Check framework registration or reflection where indicated. Distinguish immutable inputs from owned mutable state, resources, identity, or background work. Missing callers in a diff do not prove no callers exist.

## Report when

Report when inspected consumers need only functions or data and no object responsibility requires the class. State what the class actually supplies. Here the calculator merely captures a multiplication parameter, and the quote merely carries values. A short scope alone proves nothing; useful resource owners can also be local.

## Suggest

Use functions for calculations and records for data:

```fsharp
module Price =
    let gross taxRate net = net * (1m + taxRate)

let total taxRate (amounts: decimal list) =
    amounts |> List.sumBy (Price.gross taxRate)

type PriceQuote = { net: decimal; taxRate: decimal }
```

Choose local or module placement under the selected scope preference. Check consumers before changing class construction or equality semantics; a record introduces structural equality. This rule is the alternative to rule `classes-for-configured-logic`.

## Do not report

Keep a class that owns a resource and its disposal:

```fsharp
type TextSource(fileName: string) =
    let reader = System.IO.File.OpenText fileName

    member _.Read() = reader.ReadToEnd()

    interface System.IDisposable with
        member _.Dispose() = reader.Dispose()

let contents fileName =
    use source = new TextSource(fileName)
    source.Read()
```

The local instance manages an open reader. Also retain necessary mutable-state owners, framework-instantiated types, and C# facades covered by rule `csharp-facade`.
