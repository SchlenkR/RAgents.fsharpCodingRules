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


## Look for

FSharpLint's naming checks locate names whose casing disagrees with their kind, such as a PascalCase function and parameter:

```fsharp
let InvoiceTotal (Lines: decimal list) = List.sum Lines
```

Also look for new record fields that use a different casing from comparable records in the same code base.

## Check the context

1. Resolve each declaration's kind: module/type/member/union case versus a let-bound function/value or parameter.
2. Inspect nearby records and the repository's FSharpLint configuration for the selected field convention.
3. Check generated code, serialization/reflection contracts, overrides, and literals before renaming. Casing can be part of a runtime contract.

## Report when

This is a tool rule: have FSharpLint's configured naming rules report an owned name that violates the selected convention. Semantic reviewers skip casing findings. Record-field casing follows the established project choice, rather than a reviewer's preferred default; add missing configuration when the linter cannot express that choice yet.

## Suggest

Use camelCase for the function and its parameter:

```fsharp
let invoiceTotal (lines: decimal list) = List.sum lines
```

Keep PascalCase for namespaces, modules, types, cases, and members. Update references along with declarations; preserve externally fixed names or their explicit serialization mapping. Run FSharpLint after the change rather than performing repeated manual casing reviews.

## Do not report

Keep PascalCase literals used as constants in patterns:

```fsharp
[<Literal>]
let MaxRetries = 3

let isFinalAttempt attempt =
    match attempt with
    | MaxRetries -> true
    | _ -> false
```

Here PascalCase lets the pattern denote the constant rather than bind a new variable. Names fixed by a schema, reflection consumer, or framework contract are also exempt when the contract is established.
