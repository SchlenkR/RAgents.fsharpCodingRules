---
id: casing-conventions
category: naming
check: tool
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Microsoft F# component design guidelines, naming conventions: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
  - "G-Research F# formatting conventions, naming conventions: https://github.com/G-Research/fsharp-formatting-conventions"
  - "FSharpLint naming rules: https://fsprojects.github.io/FSharpLint/how-tos/rule-configuration.html"
---

# Follow the F# casing conventions

Use PascalCase for namespaces, modules, types, union cases, and members, and camelCase for let-bound values, functions, and parameters; choose one casing for record fields per code base and keep it. FSharpLint's naming rules check this, and consistent casing tells the reader the kind of a name before its definition.

## Check

1. For every new name: does its casing match its kind?
2. Do new record fields follow the casing of the existing records? PascalCase is the common convention; camelCase is common where records mirror JSON.

## Report

The name in the matching casing.

## Do not report

- Names fixed by an external contract, such as JSON properties read by reflection.
- `[<Literal>]` values used unqualified in patterns; only PascalCase makes them match as constants.

## Examples (illustrative, not exhaustive)

```fsharp
let InvoiceTotal (Lines: InvoiceLine list) = ... // finding: function and parameter in PascalCase
let invoiceTotal (lines: InvoiceLine list) = ... // correction
```

No finding: `[<Literal>] let MaxRetries = 3`.
