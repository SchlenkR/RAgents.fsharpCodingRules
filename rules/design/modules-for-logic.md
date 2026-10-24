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

Write logic as functions in modules over records and unions, and define a class only when something owns mutable state, a resource, or a lifetime, or when a framework requires one. A class without such state is a C# habit: it adds a constructor, instances, and injection without adding anything a function lacks.

## Check

1. For every new class: does it hold mutable state, a resource (`IDisposable`), or a background process, or does a framework require it?
2. Look for stateless service classes whose constructor only stores dependencies or settings and whose members are pure logic.
3. Look for classes that only carry data, where a record would do.

## Report

The class and its replacement: a module with functions that take dependencies as parameters, or a record.

## Do not report

- Types a framework instantiates, such as hosted services or controllers.
- C#-facing facades (rule `csharp-facade`).

## Examples (illustrative, not exhaustive)

```fsharp
type PriceCalculator(taxRate: decimal) = // finding: no state, no resource
    member _.Gross(net: decimal) = net * (1m + taxRate)

// correction
module Price =
    let gross (taxRate: decimal) (net: decimal) = net * (1m + taxRate)
```

No finding: a `RateLimiter` class with a `let mutable` token count, a timer, and `IDisposable`.
